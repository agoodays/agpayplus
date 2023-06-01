using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Extensions;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay;
using AGooday.AgPay.Payment.Api.Channel.WxPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using AGooday.AgPay.Payment.Api.Channel.YsfPay;
using AGooday.AgPay.Payment.Api.Extensions;
using AGooday.AgPay.Payment.Api.FilterAttributes;
using AGooday.AgPay.Payment.Api.Jobs;
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

#region PayWay
#region AliPay
using AliBar = AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay.AliBar;
using AliJsapi = AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay.AliJsapi;
#endregion
#region WxPay
using WxApp = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay.WxApp;
using WxBar = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay.WxBar;
using WxH5 = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay.WxH5;
using WxJsapi = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay.WxJsapi;
using WxLite = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay.WxLite;
using WxNative = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay.WxNative;

using WxAppV3 = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3.WxApp;
using WxBarV3 = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3.WxBar;
using WxH5V3 = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3.WxH5;
using WxJsapiV3 = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3.WxJsapi;
using WxLiteV3 = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3.WxLite;
using WxNativeV3 = AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3.WxNative;
#endregion
#region YsfPay
using YsfAliBar = AGooday.AgPay.Payment.Api.Channel.YsfPay.PayWay.AliBar;
using YsfAliJsapi = AGooday.AgPay.Payment.Api.Channel.YsfPay.PayWay.AliJsapi;

using YsfWxBar = AGooday.AgPay.Payment.Api.Channel.YsfPay.PayWay.WxBar;
using YsfWxJsapi = AGooday.AgPay.Payment.Api.Channel.YsfPay.PayWay.WxJsapi;
#endregion
#endregion

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

#region MQ
var mqconfiguration = builder.Configuration.GetSection("MQ:RabbitMQ");
services.Configure<RabbitMQConfiguration>(mqconfiguration);
#endregion

#region OSS
builder.Configuration.GetSection("OSS").Bind(LocalOssConfig.Oss);
builder.Configuration.GetSection("OSS:AliyunOss").Bind(AliyunOssConfig.Oss);
#endregion

services.AddMemoryCache();

// Automapper 注入
services.AddAutoMapperSetup();
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

// Newtonsoft.Json 全部配置 
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    DateFormatString = "yyyy-MM-dd HH:mm:ss",
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

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
services.AddSingleton<RequestIpUtil>();
// .NET Core 原生依赖注入
// 单写一层用来添加依赖项，从展示层 Presentation 中隔离
NativeInjectorBootStrapper.RegisterServices(services);

#region RabbitMQ
services.AddSingleton<IMQSender, RabbitMQSender>();
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
services.AddHostedService<RabbitListener>();
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
services.AddSingleton<ChannelOrderReissueService>();
services.AddSingleton<ConfigContextQueryService>();
services.AddSingleton<ConfigContextService>();
services.AddSingleton<PayMchNotifyService>();
services.AddSingleton<PayOrderDivisionProcessService>();
services.AddSingleton<PayOrderProcessService>();
services.AddSingleton<RefundOrderProcessService>();
#region DivisionService
//services.AddSingleton<IDivisionService, AliPayDivisionService>();
//services.AddSingleton<IDivisionService, WxPayDivisionService>();
services.AddSingleton<AliPayDivisionService>();
services.AddSingleton<WxPayDivisionService>();
services.AddSingleton(provider =>
{
    Func<string, IDivisionService> funcFactory = ifCode =>
    {
        switch (ifCode)
        {
            case CS.IF_CODE.ALIPAY:
                return provider.GetService<AliPayDivisionService>();
            case CS.IF_CODE.WXPAY:
                return provider.GetService<WxPayDivisionService>();
            default:
                return null;
        }
    };
    return funcFactory;
});
#endregion
#region PaymentService
services.AddSingleton<AliPayPaymentService>();
services.AddSingleton<WxPayPaymentService>();
services.AddSingleton<YsfPayPaymentService>();
services.AddSingleton(provider =>
{
    Func<string, IPaymentService> funcFactory = ifCode =>
    {
        switch (ifCode)
        {
            case CS.IF_CODE.ALIPAY:
                return provider.GetService<AliPayPaymentService>();
            case CS.IF_CODE.WXPAY:
                return provider.GetService<WxPayPaymentService>();
            case CS.IF_CODE.YSFPAY:
                return provider.GetService<YsfPayPaymentService>();
            default:
                return null;
        }
    };
    return funcFactory;
});
#endregion
#region RefundService
services.AddSingleton<AliPayRefundService>();
services.AddSingleton<WxPayRefundService>();
services.AddSingleton<YsfPayRefundService>();
services.AddSingleton(provider =>
{
    Func<string, IRefundService> funcFactory = ifCode =>
    {
        switch (ifCode)
        {
            case CS.IF_CODE.ALIPAY:
                return provider.GetService<AliPayRefundService>();
            case CS.IF_CODE.WXPAY:
                return provider.GetService<WxPayRefundService>();
            case CS.IF_CODE.YSFPAY:
                return provider.GetService<YsfPayRefundService>();
            default:
                return null;
        }
    };
    return funcFactory;
});
#endregion
#region ChannelNoticeService
services.AddSingleton<AliPayChannelNoticeService>();
services.AddSingleton<WxPayChannelNoticeService>();
services.AddSingleton<YsfPayChannelNoticeService>();
services.AddSingleton(provider =>
{
    Func<string, IChannelNoticeService> funcFactory = ifCode =>
    {
        switch (ifCode)
        {
            case CS.IF_CODE.ALIPAY:
                return provider.GetService<AliPayChannelNoticeService>();
            case CS.IF_CODE.WXPAY:
                return provider.GetService<WxPayChannelNoticeService>();
            case CS.IF_CODE.YSFPAY:
                return provider.GetService<YsfPayChannelNoticeService>();
            default:
                return null;
        }
    };
    return funcFactory;
});
#endregion
#region QueryService
services.AddSingleton<AliPayPayOrderQueryService>();
services.AddSingleton<WxPayPayOrderQueryService>();
services.AddSingleton<YsfPayPayOrderQueryService>();
services.AddSingleton(provider =>
{
    Func<string, IPayOrderQueryService> funcFactory = ifCode =>
    {
        switch (ifCode)
        {
            case CS.IF_CODE.ALIPAY:
                return provider.GetService<AliPayPayOrderQueryService>();
            case CS.IF_CODE.WXPAY:
                return provider.GetService<WxPayPayOrderQueryService>();
            case CS.IF_CODE.YSFPAY:
                return provider.GetService<YsfPayPayOrderQueryService>();
            default:
                return null;
        }
    };
    return funcFactory;
});
#endregion
#region AliPay
services.AddSingleton<IPaymentService, AliApp>();
services.AddSingleton<IPaymentService, AliBar>();
services.AddSingleton<IPaymentService, AliJsapi>();
services.AddSingleton<IPaymentService, AliPc>();
services.AddSingleton<IPaymentService, AliQr>();
services.AddSingleton<IPaymentService, AliWap>();
#endregion
#region WxPay
services.AddSingleton<IPaymentService, WxApp>();
services.AddSingleton<IPaymentService, WxBar>();
services.AddSingleton<IPaymentService, WxH5>();
services.AddSingleton<IPaymentService, WxJsapi>();
services.AddSingleton<IPaymentService, WxLite>();
services.AddSingleton<IPaymentService, WxNative>();

services.AddSingleton<IPaymentService, WxAppV3>();
services.AddSingleton<IPaymentService, WxBarV3>();
services.AddSingleton<IPaymentService, WxH5V3>();
services.AddSingleton<IPaymentService, WxJsapiV3>();
services.AddSingleton<IPaymentService, WxLiteV3>();
services.AddSingleton<IPaymentService, WxNativeV3>();
#endregion
#region YsfPay
services.AddSingleton<IPaymentService, YsfAliBar>();
services.AddSingleton<IPaymentService, YsfAliJsapi>();

services.AddSingleton<IPaymentService, YsfWxBar>();
services.AddSingleton<IPaymentService, YsfWxJsapi>();
#endregion
#region ChannelUserService
services.AddSingleton<AliPayChannelUserService>();
services.AddSingleton<WxPayChannelUserService>();
services.AddSingleton(provider =>
{
    Func<string, IChannelUserService> funcFactory = ifCode =>
    {
        switch (ifCode)
        {
            case CS.IF_CODE.ALIPAY:
                return provider.GetService<AliPayChannelUserService>();
            case CS.IF_CODE.WXPAY:
                return provider.GetService<WxPayChannelUserService>();
            default:
                return null;
        }
    };
    return funcFactory;
});
#endregion

services.AddSingleton<IQRCodeService, QRCodeService>();

var serviceProvider = services.BuildServiceProvider();
PayWayUtil.ServiceProvider = serviceProvider;
AliPayKit.ServiceProvider = serviceProvider;
WxPayKit.ServiceProvider = serviceProvider;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
