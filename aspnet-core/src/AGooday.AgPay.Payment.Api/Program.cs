using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Extensions;
using AGooday.AgPay.Components.Cache.Options;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Extensions;
using AGooday.AgPay.Components.SMS.Extensions;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Extensions;
using AGooday.AgPay.Payment.Api.FilterAttributes;
using AGooday.AgPay.Payment.Api.Jobs;
using AGooday.AgPay.Payment.Api.Middlewares;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.MQ;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Mvc;
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

services.AddSingleton(new Appsettings(Env.ContentRootPath));

//// 注入日志
//services.AddLogging(config =>
//{
//    //Microsoft.Extensions.Logging.Log4Net.AspNetCore
//    config.AddLog4Net();
//});
services.AddSingleton<ILoggerProvider, Log4NetLoggerProvider>();

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

services.AddMemoryCache();

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

services.AddControllersWithViews()
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
services.AddSwaggerGen();

// Adding MediatR for Domain Events
// 领域命令、领域事件等注入
// 引用包 MediatR.Extensions.Microsoft.DependencyInjection
//services.AddMediatR(typeof(MyxxxHandler));//单单注入某一个处理程序
//或
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());//目的是为了扫描Handler的实现对象并添加到IOC的容器中

services.Configure<ApiBehaviorOptions>(options =>
{
    // 禁用默认模型验证过滤器
    options.SuppressModelStateInvalidFilter = true;
});
services.Configure<MvcOptions>(options =>
{
    // 全局添加自定义模型验证过滤器
    options.Filters.Add<ValidateModelAttribute>();
});

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//services.AddSingleton<RequestIpUtil>();
services.AddSingleton<RequestKit>();

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
    typeof(PayOrderDivisionRabbitMQReceiver),
    typeof(PayOrderMchNotifyRabbitMQReceiver),
    typeof(PayOrderReissueRabbitMQReceiver),
    typeof(ResetAppConfigRabbitMQReceiver),
    typeof(ResetIsvAgentMchAppInfoRabbitMQReceiver)
};

foreach (var type in receiverTypes)
{
    services.AddSingleton(typeof(IMQMsgReceiver), type);
}

var specificReceiverTypes = new[]
{
    (typeof(PayOrderDivisionMQ.IMQReceiver), typeof(PayOrderDivisionMQReceiver)),
    (typeof(PayOrderMchNotifyMQ.IMQReceiver), typeof(PayOrderMchNotifyMQReceiver)),
    (typeof(PayOrderReissueMQ.IMQReceiver), typeof(PayOrderReissueMQReceiver)),
    (typeof(ResetAppConfigMQ.IMQReceiver), typeof(ResetAppConfigMQReceiver)),
    (typeof(ResetIsvAgentMchAppInfoConfigMQ.IMQReceiver), typeof(ResetIsvAgentMchAppInfoMQReceiver))
};

foreach (var (serviceType, implementationType) in specificReceiverTypes)
{
    services.AddSingleton(serviceType, implementationType);
}
//services.AddSingleton<IMQMsgReceiver, PayOrderDivisionRabbitMQReceiver>();
//services.AddSingleton<IMQMsgReceiver, PayOrderMchNotifyRabbitMQReceiver>();
//services.AddSingleton<IMQMsgReceiver, PayOrderReissueRabbitMQReceiver>();
//services.AddSingleton<IMQMsgReceiver, ResetAppConfigRabbitMQReceiver>();
//services.AddSingleton<IMQMsgReceiver, ResetIsvAgentMchAppInfoRabbitMQReceiver>();
//services.AddSingleton<PayOrderDivisionMQ.IMQReceiver, PayOrderDivisionMQReceiver>();
//services.AddSingleton<PayOrderMchNotifyMQ.IMQReceiver, PayOrderMchNotifyMQReceiver>();
//services.AddSingleton<PayOrderReissueMQ.IMQReceiver, PayOrderReissueMQReceiver>();
//services.AddSingleton<ResetAppConfigMQ.IMQReceiver, ResetAppConfigMQReceiver>();
//services.AddSingleton<ResetIsvAgentMchAppInfoConfigMQ.IMQReceiver, ResetIsvAgentMchAppInfoMQReceiver>();
// 注册 HostedService
services.AddHostedService<MQReceiverHostedService>();
#endregion

#region Quartz
// 添加 Quartz 任务
services.AddQuartzJobs(builder.Configuration);
#endregion

#region OSS
OSSNativeInjectorBootStrapper.RegisterServices(services);
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

#region SMS
SMSNativeInjectorBootStrapper.RegisterServices(services);
#endregion

services.AddSingleton<ChannelCertConfigKit>(serviceProvider =>
{
    return new ChannelCertConfigKit(serviceProvider);
});

//var provider = services.BuildServiceProvider();
//var mchAppService = (IMchAppService)provider.GetService(typeof(IMchAppService));
//var mchInfoService = (IMchInfoService)provider.GetService(typeof(IMchInfoService));
//var isvInfoService = (IIsvInfoService)provider.GetService(typeof(IIsvInfoService));
//var payInterfaceConfigService = (IPayInterfaceConfigService)provider.GetService(typeof(IPayInterfaceConfigService));
//services.AddSingleton(new ConfigContextService(mchAppService, mchInfoService, isvInfoService, payInterfaceConfigService));
//services.AddScoped<ConfigContextService>(provider =>
//{
//    var mchStoreService = (IMchStoreService)provider.GetService(typeof(IMchStoreService));
//    var mchAppService = (IMchAppService)provider.GetService(typeof(IMchAppService));
//    var mchInfoService = (IMchInfoService)provider.GetService(typeof(IMchInfoService));
//    var agentInfoService = (IAgentInfoService)provider.GetService(typeof(IAgentInfoService));
//    var isvInfoService = (IIsvInfoService)provider.GetService(typeof(IIsvInfoService));
//    var payInterfaceConfigService = (IPayInterfaceConfigService)provider.GetService(typeof(IPayInterfaceConfigService));
//    return new ConfigContextService(mchStoreService, mchAppService, mchInfoService, agentInfoService, isvInfoService, payInterfaceConfigService);
//});
//services.AddSingleton(typeof(ConfigContextService));
//services.AddSingleton(typeof(ConfigContextQueryService));
services.AddSingleton<ConfigContextService>();
services.AddScoped<ConfigContextQueryService>();
services.AddScoped<ChannelOrderReissueService>();
services.AddScoped<PayMchNotifyService>();
services.AddScoped<PayOrderDivisionProcessService>();
services.AddScoped<PayOrderProcessService>();
services.AddScoped<RefundOrderProcessService>();

services.AddSingleton<IQRCodeService, QRCodeService>();

ChannelNativeInjectorBootStrapper.RegisterServices(services);

AgPayUtil.AES_KEY = builder.Configuration["AesKey"];
AgPayUtil.RSA2_PRIVATE_KEY = builder.Configuration["SysRSA2:PrivateKey"];

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

// 绑定配置
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

// 读取配置
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

// 自定义中间件
app.UseNdc();
app.UseCalculateExecutionTime();
app.UseRequestResponseLogging();

// Swagger 文档（开发环境下）
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
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

// 授权中间件（检测用户是否有权限访问资源）
app.UseAuthorization();

// 异常处理中间件（放在路由中间件之前）
app.UseExceptionHandling();

// 路由映射
app.UseRouting().UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    // 配置默认路由，将所有非 API 路由重定向到 Vue 单页面应用程序的入口页
    endpoints.MapFallbackToFile("/cashier/index.html");

    //// 配置 Vue 单页面应用程序的路由
    //endpoints.MapGet("/", async context =>
    //{
    //    context.Response.Redirect("/cashier/index.html");
    //});
});

app.Run();
