using AGooday.AgPay.Base.Api.Extensions;
using AGooday.AgPay.Base.Api.Hosting;
using AGooday.AgPay.Base.Api.MQ;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Extensions;
using AGooday.AgPay.Payment.Api.FilterAttributes;
using AGooday.AgPay.Payment.Api.Jobs;
// 注意：ResetIsvAgentMchAppInfoMQReceiver 虽声明在本命名空间下，但物理文件位于 Base.Api 项目
using AGooday.AgPay.Payment.Api.MQ;
using AGooday.AgPay.Payment.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 共享宿主：启动管线与横切配置的唯一实现（原 4 份复制粘贴的 Program.cs 已收敛到此）
var hostOptions = new AgPayApiHostOptions
{
    ApplicationAssembly = typeof(Program).Assembly,
    SystemName = "Payment",
    // Payment.Api 不使用 JWT：商户接口依赖请求验签，且 Swagger 无安全定义
    UseJwtAuthentication = false,
    UseChannelConfigContext = true,
    // 保持现状：Payment.Api 使用 AddControllersWithViews 并托管收银台 SPA
    UseControllersWithViews = true,
    MapCashierFallback = true,
    EnableQuartzHealthCheck = true,
    InitializeIdWorker = true,
    SuppressModelStateInvalidFilter = true,
    ConfigureMvcOptions = mvcOptions => mvcOptions.Filters.Add<ValidateModelAttribute>(),
};
builder.AddAgPayApiHost(hostOptions);

// ==================== 以下为 Payment.Api 独有注册 ====================
var services = builder.Services;

// 【既有缺陷修复】ApplymentReissueJob 依赖 MchApplymentService（Components.Third），
// 但原 Program.cs 从未在 Payment.Api 注册它 —— 原先只有 Manager.Api 注册。
// 后果：Development 环境 ValidateOnBuild 直接启动失败；生产环境该任务每 10 分钟执行时报错。
services.AddScoped<MchApplymentService>();

services.AddSingleton<RequestKit>();

services.AddSingleton<ChannelCertConfigKit>(serviceProvider =>
{
    return new ChannelCertConfigKit(serviceProvider);
});

services.AddScoped<ChannelOrderReissueService>();
services.AddScoped<TransferOrderReissueService>();
services.AddScoped<PayMchNotifyService>();
services.AddScoped<PayOrderDivisionProcessService>();
services.AddScoped<PayOrderProcessService>();
services.AddScoped<RefundOrderProcessService>();

services.AddSingleton<IQRCodeService, QRCodeService>();

// 通知（邮件）
services.AddNotice(builder.Configuration);

#region Quartz
// 添加 Quartz 任务
services.AddQuartzJobs(builder.Configuration);
#endregion

#region RabbitMQ
// 注册 RabbitMQ 服务并动态注册 Receiver
services.AddRabbitMQServices(
    rabbitMQReceiverTypes: new[]
    {
        typeof(PayOrderDivisionRabbitMQReceiver),
        typeof(PayOrderMchNotifyRabbitMQReceiver),
        typeof(PayOrderReissueRabbitMQReceiver),
        typeof(ResetAppConfigRabbitMQReceiver),
        typeof(ResetIsvAgentMchAppInfoRabbitMQReceiver)
    },
    specificReceiverTypes: new[]
    {
        (typeof(PayOrderDivisionMQ.IMQReceiver), typeof(PayOrderDivisionMQReceiver)),
        (typeof(PayOrderMchNotifyMQ.IMQReceiver), typeof(PayOrderMchNotifyMQReceiver)),
        (typeof(PayOrderReissueMQ.IMQReceiver), typeof(PayOrderReissueMQReceiver)),
        (typeof(ResetAppConfigMQ.IMQReceiver), typeof(ResetAppConfigMQReceiver)),
        (typeof(ResetIsvAgentMchAppInfoConfigMQ.IMQReceiver), typeof(ResetIsvAgentMchAppInfoMQReceiver)),
        (typeof(ApplymentChannelResultMQ.IMQReceiver), typeof(ApplymentChannelResultMQReceiver))
    }
);
#endregion

var app = builder.Build();

// 共享中间件管线
app.UseAgPayApiHost(hostOptions);

app.Run();
