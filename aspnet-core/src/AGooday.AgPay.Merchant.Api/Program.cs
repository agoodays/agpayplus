using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Merchant.Api.Authorization;
using AGooday.AgPay.Merchant.Api.Extensions;
using AGooday.AgPay.Merchant.Api.Extensions.AuthContext;
using AGooday.AgPay.Merchant.Api.Filter;
using AGooday.AgPay.Merchant.Api.Logs;
using AGooday.AgPay.Merchant.Api.Middlewares;
using AGooday.AgPay.Merchant.Api.Models;
using AGooday.AgPay.Merchant.Api.MQ;
using AGooday.AgPay.Merchant.Api.WebSockets;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var logging = builder.Logging;
// 调用 ClearProviders 以从生成器中删除所有 ILoggerProvider 实例
logging.ClearProviders();
//// 通常，日志级别应在配置中指定，而不是在代码中指定。
//logging.AddFilter("Microsoft", LogLevel.Warning);
// 添加控制台日志记录提供程序。
logging.AddConsole();

// Add services to the container.
var services = builder.Services;
var Env = builder.Environment;

//用户信息
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

services.AddSingleton(new Appsettings(Env.ContentRootPath));

//// 注入日志
//services.AddLogging(config =>
//{
//    //Microsoft.Extensions.Logging.Log4Net.AspNetCore
//    config.AddLog4Net();
//});
services.AddSingleton<ILoggerProvider, Log4NetLoggerProvider>();

services.AddScoped<ILogHandler, LogHandler>();

#region Redis
//redis缓存
var section = builder.Configuration.GetSection("Redis:Default");
//连接字符串
string _connectionString = section.GetSection("Connection").Value;
//实例名称
string _instanceName = section.GetSection("InstanceName").Value;
//默认数据库 
int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
services.AddSingleton(new RedisUtil(_connectionString, _instanceName, _defaultDB));
#endregion

#region MQ
var mqconfiguration = builder.Configuration.GetSection("MQ:RabbitMQ");
services.Configure<RabbitMQConfiguration>(mqconfiguration);
#endregion

var cors = builder.Configuration.GetSection("Cors").Value;
services.AddCors(o =>
    o.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins(cors.Split(","))
            .AllowAnyHeader()
            .AllowAnyMethod()
            //.AllowAnyOrigin()
            .AllowCredentials()
    ));

services.AddMemoryCache();
services.AddHttpContextAccessor();
services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
var jwtSettingsSection = builder.Configuration.GetSection("JWT");
services.Configure<JwtSettings>(jwtSettingsSection);
// JWT
var appSettings = jwtSettingsSection.Get<JwtSettings>();
services.AddJwtBearerAuthentication(appSettings);

services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Automapper 注入
services.AddAutoMapperSetup();
services.AddControllersWithViews(options =>
{
    //日志过滤器
    options.Filters.Add<LogActionFilter>();
})
    //.AddNewtonsoftJson();
    .AddNewtonsoftJson(options =>
    {
        //https://blog.poychang.net/using-newtonsoft-json-in-asp-net-core-projects/
        //options.SerializerSettings.Formatting = Formatting.Indented;
        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//Json key 首字符小写（大驼峰转小驼峰）
    });

// Newtonsoft.Json 全部配置 
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    //Formatting = Formatting.Indented,
    DateFormatString = "yyyy-MM-dd HH:mm:ss",
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AGooday.AgPay.Merchant.Api", Version = "1.0" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = $"JWT Authorization header using the Bearer scheme. \r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
    options.OperationFilter<SwaggerSecurityScheme>();
});

// Adding MediatR for Domain Events
// 领域命令、领域事件等注入
// 引用包 MediatR.Extensions.Microsoft.DependencyInjection
//services.AddMediatR(typeof(MyxxxHandler));//单单注入某一个处理程序
//或
services.AddMediatR(typeof(Program));//目的是为了扫描Handler的实现对象并添加到IOC的容器中

// .NET Core 原生依赖注入
// 单写一层用来添加依赖项，从展示层 Presentation 中隔离
NativeInjectorBootStrapper.RegisterServices(services);

#region RabbitMQ
services.AddSingleton<IMQSender, RabbitMQSender>();
services.AddScoped<IMQMsgReceiver, ResetAppConfigRabbitMQReceiver>();
services.AddScoped<IMQMsgReceiver, CleanMchLoginAuthCacheRabbitMQReceiver>();
services.AddScoped<CleanMchLoginAuthCacheMQ.IMQReceiver, CleanMchLoginAuthCacheMQReceiver>();
services.AddScoped<ResetAppConfigMQ.IMQReceiver, ResetAppConfigMQReceiver>();
services.AddHostedService<RabbitListener>();
#endregion

//加入 WebSocket 处理服务
builder.Services.AddSingleton<WsChannelUserIdServer>();
builder.Services.AddSingleton<WsPayOrderServer>();

var app = builder.Build();

//加入 WebSocket 功能
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

// Configure the HTTP request pipeline.
app.UseCalculateExecutionTime();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseCors("CorsPolicy");

var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
AuthContextService.Configure(httpContextAccessor);

app.UseAuthorization();

app.UseExceptionHandling();

//app.UseRequestResponseLogging();

app.MapControllers();

app.Run();
