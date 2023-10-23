using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Extensions;
using AGooday.AgPay.Payment.Api.Channel.Extensions;
using AGooday.AgPay.Payment.Api.Extensions;
using AGooday.AgPay.Payment.Api.FilterAttributes;
using AGooday.AgPay.Payment.Api.Jobs;
using AGooday.AgPay.Payment.Api.Middlewares;
using AGooday.AgPay.Payment.Api.MQ;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

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
        options.SerializerSettings.Formatting = Formatting.Indented;
        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//Json key 首字符小写（大驼峰转小驼峰）
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Adding MediatR for Domain Events
// 领域命令、领域事件等注入
// 引用包 MediatR.Extensions.Microsoft.DependencyInjection
//services.AddMediatR(typeof(MyxxxHandler));//单单注入某一个处理程序
//或
services.AddMediatR(typeof(Program));//目的是为了扫描Handler的实现对象并添加到IOC的容器中

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

#region RabbitMQ
services.AddTransient<RabbitMQSender>();
services.AddSingleton<IMQSender>(provider =>
{
    var mqSenderFactory = new MQSenderFactory(builder.Configuration, provider);
    return mqSenderFactory.CreateSender();
});
services.AddSingleton<IMQMsgReceiver, PayOrderDivisionRabbitMQReceiver>();
services.AddSingleton<IMQMsgReceiver, PayOrderMchNotifyRabbitMQReceiver>();
services.AddSingleton<IMQMsgReceiver, PayOrderReissueRabbitMQReceiver>();
services.AddSingleton<IMQMsgReceiver, ResetAppConfigRabbitMQReceiver>();
services.AddSingleton<IMQMsgReceiver, ResetIsvMchAppInfoRabbitMQReceiver>();
services.AddSingleton<PayOrderDivisionMQ.IMQReceiver, PayOrderDivisionMQReceiver>();
services.AddSingleton<PayOrderMchNotifyMQ.IMQReceiver, PayOrderMchNotifyMQReceiver>();
services.AddSingleton<PayOrderReissueMQ.IMQReceiver, PayOrderReissueMQReceiver>();
services.AddSingleton<ResetAppConfigMQ.IMQReceiver, ResetAppConfigMQReceiver>();
services.AddSingleton<ResetIsvMchAppInfoConfigMQ.IMQReceiver, ResetIsvMchAppInfoMQReceiver>();
services.AddHostedService<MQReceiverHostedService>();
#endregion

#region Quartz
/*
 简单说一下Cron表达式吧，

 由7段构成：秒 分 时 日 月 星期 年（可选）

 "-" ：表示范围  MON-WED表示星期一到星期三
 "," ：表示列举 MON,WEB表示星期一和星期三
 "*" ：表是“每”，每月，每天，每周，每年等
 "/" :表示增量：0/15（处于分钟段里面） 每15分钟，在0分以后开始，3/20 每20分钟，从3分钟以后开始
 "?" :只能出现在日，星期段里面，表示不指定具体的值
 "L" :只能出现在日，星期段里面，是Last的缩写，一个月的最后一天，一个星期的最后一天（星期六）
 "W" :表示工作日，距离给定值最近的工作日
 "#" :表示一个月的第几个星期几，例如："6#3"表示每个月的第三个星期五（1=SUN...6=FRI,7=SAT）

 如果Minutes的数值是 '0/15' ，表示从0开始每15分钟执行

 如果Minutes的数值是 '3/20' ，表示从3开始每20分钟执行，也就是‘3/23/43’
*/
// https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
// 添加Quartz服务
services.AddSingleton<IJobFactory, SingletonJobFactory>();
services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

// 添加任务
services.AddSingleton<PayOrderExpiredJob>();
services.AddSingleton<PayOrderReissueJob>();
services.AddSingleton<RefundOrderExpiredJob>();
services.AddSingleton<RefundOrderReissueJob>();
services.AddSingleton<TransferOrderReissueJob>();
services.AddSingleton(new JobSchedule(
    jobType: typeof(PayOrderExpiredJob),
    cronExpression: "0 0/1 * * * ?")); // 每分钟执行一次
services.AddSingleton(new JobSchedule(
    jobType: typeof(PayOrderReissueJob),
    cronExpression: "0 0/1 * * * ?")); // 每分钟执行一次
services.AddSingleton(new JobSchedule(
    jobType: typeof(RefundOrderExpiredJob),
    cronExpression: "0 0/1 * * * ?")); // 每分钟执行一次
services.AddSingleton(new JobSchedule(
    jobType: typeof(RefundOrderReissueJob),
    cronExpression: "0 0/1 * * * ?")); // 每分钟执行一次
services.AddSingleton(new JobSchedule(
    jobType: typeof(TransferOrderReissueJob),
    cronExpression: "0 0/1 * * * ?")); // 每分钟执行一次

services.AddHostedService<QuartzHostedService>();
#endregion

#region OSS
OSSNativeInjectorBootStrapper.RegisterServices(services);
#endregion

services.AddSingleton<ChannelCertConfigKit>(serviceProvider =>
{
    var ossServiceFactory = serviceProvider.GetService<IOssServiceFactory>();
    return new ChannelCertConfigKit(ossServiceFactory);
});

//var provider = services.BuildServiceProvider();
//var mchAppService = (IMchAppService)provider.GetService(typeof(IMchAppService));
//var mchInfoService = (IMchInfoService)provider.GetService(typeof(IMchInfoService));
//var isvInfoService = (IIsvInfoService)provider.GetService(typeof(IIsvInfoService));
//var payInterfaceConfigService = (IPayInterfaceConfigService)provider.GetService(typeof(IPayInterfaceConfigService));
//services.AddSingleton(new ConfigContextService(mchAppService, mchInfoService, isvInfoService, payInterfaceConfigService));
//services.AddSingleton(typeof(ConfigContextQueryService));
//services.AddSingleton(typeof(ConfigContextService));
services.AddScoped<ConfigContextService>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCalculateExecutionTime();

app.UseRequestResponseLogging();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.UseExceptionHandling();

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
