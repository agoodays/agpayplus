using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Extensions;
using AGooday.AgPay.Components.Cache.Options;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Components.OCR.Controllers;
using AGooday.AgPay.Components.OCR.Extensions;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Controllers;
using AGooday.AgPay.Components.OSS.Extensions;
using AGooday.AgPay.Components.SMS.Extensions;
using AGooday.AgPay.Manager.Api.Authorization;
using AGooday.AgPay.Manager.Api.Extensions;
using AGooday.AgPay.Manager.Api.Extensions.AuthContext;
using AGooday.AgPay.Manager.Api.Filter;
using AGooday.AgPay.Manager.Api.Middlewares;
using AGooday.AgPay.Manager.Api.Models;
using AGooday.AgPay.Manager.Api.MQ;
using AGooday.AgPay.Manager.Api.OpLog;
using AGooday.AgPay.Manager.Api.WebSockets;
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

//services.AddSingleton(new Appsettings(Env.ContentRootPath));
services.AddSingleton(new Appsettings(builder.Configuration));

//用户信息
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//// 注入日志
//services.AddLogging(config =>
//{
//    //Microsoft.Extensions.Logging.Log4Net.AspNetCore
//    config.AddLog4Net();
//});
services.AddSingleton<ILoggerProvider, Log4NetLoggerProvider>();

services.AddScoped<IOpLogHandler, OpLogHandler>();

#region Redis
var redisSettingsSection = builder.Configuration.GetSection("Redis:Default");
var redisOptions = redisSettingsSection.Get<RedisOptions>();

//services.AddSingleton(new RedisUtil(redisOptions.Connection, redisOptions.InstanceName, redisOptions.DefaultDB));

CacheNativeInjectorBootStrapper.RegisterServices(services, redisOptions);
#endregion

#region OSS
builder.Configuration.GetSection("OSS").Bind(LocalOssConfig.Oss);
builder.Configuration.GetSection("OSS:AliyunOss").Bind(AliyunOssConfig.Oss);
#endregion

#region CORS
// 从 appsettings.json 中读取 CORS 配置
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
// 添加 CORS 服务
services.AddCors(o =>
    o.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins(allowedOrigins)
            //.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    ));
#endregion

services.AddMemoryCache();
services.AddHttpContextAccessor();
services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
var jwtSettingsSection = builder.Configuration.GetSection("JWT");
services.Configure<JwtSettings>(jwtSettingsSection);
// JWT 认证
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
services.AddJwtBearerAuthentication(jwtSettings);

services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

var sysRSA2Section = builder.Configuration.GetSection("SysRSA2");
services.Configure<SysRSA2Config>(sysRSA2Section);

AgPayUtil.AES_KEY = builder.Configuration["AesKey"];
var sysRSA2Config = sysRSA2Section.Get<SysRSA2Config>();
AgPayUtil.RSA2_PRIVATE_KEY = sysRSA2Config.PrivateKey;

// Automapper 注入
services.AddAutoMapperSetup();

// Newtonsoft.Json 全部配置 
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.None,//格式化
    DateFormatString = "yyyy-MM-dd HH:mm:ss",
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    NullValueHandling = NullValueHandling.Ignore
};

services.AddControllers(options =>
    {
        ////添加全局异常过滤器
        //options.Filters.Add<GlobalExceptionsFilter>();
        //日志过滤器
        options.Filters.Add<OpLogActionFilter>();
    })
    .AddApplicationPart(typeof(OssFileController).Assembly)
    .AddApplicationPart(typeof(OcrController).Assembly)
    //.AddNewtonsoftJson();
    .AddNewtonsoftJson(options =>
    {
        //https://blog.poychang.net/using-newtonsoft-json-in-asp-net-core-projects/
        options.SerializerSettings.Formatting = Formatting.None;
        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//Json key 首字符小写（大驼峰转小驼峰）
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.Converters.Add(new BaseModelJsonConverter<BaseModel>());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AGooday.AgPay.Manager.Api", Version = "1.0" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = $"JWT Authorization header using the Bearer scheme. \r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });

    /**
     * 修改项目文件 .csproj
     * 生成XML注释文件，以便Swagger可以读取
     * <PropertyGroup>
     *     <GenerateDocumentationFile>true</GenerateDocumentationFile>
     *     <NoWarn>$(NoWarn);1591</NoWarn>
     * </PropertyGroup>
     * 
     * 配置 Swagger 注释路径
     * var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
     * options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
     * options.OperationFilter<SwaggerSecurityScheme>();
     * **/

    //注册全局认证（所有的接口都可以使用认证）
    //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = JwtBearerDefaults.AuthenticationScheme
    //            },
    //            Scheme = "oauth2",
    //            Name = JwtBearerDefaults.AuthenticationScheme,
    //            In = ParameterLocation.Header,
    //        },
    //        new List<string>()
    //    }
    //});
});

// Adding MediatR for Domain Events
// 领域命令、领域事件等注入
// 引用包 MediatR.Extensions.Microsoft.DependencyInjection
//services.AddMediatR(typeof(MyxxxHandler));//单单注入某一个处理程序
//或
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());//目的是为了扫描Handler的实现对象并添加到IOC的容器中

// .NET Core 原生依赖注入
// 单写一层用来添加依赖项，从展示层 Presentation 中隔离
NativeInjectorBootStrapper.RegisterServices(services);

services.AddNotice(builder.Configuration);

#region RabbitMQ
services.AddTransient<RabbitMQSender>();
services.AddSingleton<IMQSenderFactory, MQSenderFactory>();
services.AddSingleton<IMQSender>(provider =>
{
    var factory = provider.GetRequiredService<IMQSenderFactory>();
    return factory.CreateSender();
});

// 动态注册 Receiver
var receiverTypes = new[]
{
    typeof(ResetAppConfigRabbitMQReceiver)
};

foreach (var type in receiverTypes)
{
    services.AddSingleton(typeof(IMQMsgReceiver), type);
}

var specificReceiverTypes = new[]
{
    (typeof(ResetAppConfigMQ.IMQReceiver), typeof(ResetAppConfigMQReceiver))
};

foreach (var (serviceType, implementationType) in specificReceiverTypes)
{
    services.AddSingleton(serviceType, implementationType);
}
//services.AddSingleton<IMQMsgReceiver, ResetAppConfigRabbitMQReceiver>();
//services.AddSingleton<ResetAppConfigMQ.IMQReceiver, ResetAppConfigMQReceiver>();
// 注册 HostedService
services.AddHostedService<MQReceiverHostedService>();
#endregion

#region OSS
OSSNativeInjectorBootStrapper.RegisterServices(services);
#endregion

#region OCR
OCRNativeInjectorBootStrapper.RegisterServices(services);
#endregion

#region SMS
SMSNativeInjectorBootStrapper.RegisterServices(services);
#endregion

// 从配置中读取 IdWorkerConfig
var idWorkerConfig = builder.Configuration.GetSection("IdWorkerConfig");
bool isUseSnowflakeId = idWorkerConfig.GetValue<bool>("IsUseSnowflakeId", false);
long dataCenterId = idWorkerConfig.GetValue<long>("DataCenterId", 0);
long machineId = idWorkerConfig.GetValue<long>("MachineId", 0);
if (isUseSnowflakeId)
{
    // 初始化 IdWorker
    //var idWorker = new IdWorker(dataCenterId, machineId);
    //typeof(IdWorker).GetField("lazy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
    //    .SetValue(null, new Lazy<IdWorker>(() => idWorker));
    IdWorker.Initialize(dataCenterId, machineId);
}
// 初始化 SeqUtil
SeqUtil.Initialize(isUseSnowflakeId);

//加入 WebSocket 处理服务
services.AddSingleton<WsChannelUserIdServer>();

// 绑定配置
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

// 读取配置
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

// 加入 WebSocket 功能
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

// 自定义中间件
app.UseNdc();
app.UseCalculateExecutionTime();
app.UseRequestResponseLogging();

// Swagger 文档（开发环境下）
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//dotnet add package IGeekFan.AspNetCore.Knife4jUI
//app.UseKnife4UI(c =>
//{
//    c.RoutePrefix = ""; // serve the UI at root --knife4j
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
//});
//}

// 根据配置决定是否启用 HTTPS 重定向
if (appSettings.ForceHttpsRedirection)
{
    // 强制 HTTPS 重定向
    app.UseHttpsRedirection();
}

// 静态文件服务
app.UseStaticFiles();

// 启用 CORS 中间件
app.UseCors("CorsPolicy");

// 认证中间件（检测用户是否登录）
app.UseAuthentication();

// 配置 HttpContext 访问器
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
AuthContextService.Configure(httpContextAccessor);

// 授权中间件（检测用户是否有权限访问资源）
app.UseAuthorization();

// 异常处理中间件（放在路由中间件之前）
app.UseExceptionHandling();

// 路由映射
app.MapControllers();

app.Run();
