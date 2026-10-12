using AGooday.AgPay.Base.Api.Extensions;
using AGooday.AgPay.Base.Api.Hosting;
using AGooday.AgPay.Base.Api.MQ;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Manager.Api.Extensions;
using AGooday.AgPay.Manager.Api.OpLog;
using AGooday.AgPay.Manager.Api.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// 共享宿主：启动管线与横切配置的唯一实现（原 4 份复制粘贴的 Program.cs 已收敛到此）
var hostOptions = new AgPayApiHostOptions
{
    ApplicationAssembly = typeof(Program).Assembly,
    SystemName = "Manager",
    SwaggerTitle = "AGooday.AgPay.Manager.Api",
    UseJwtAuthentication = true,
    OperationLogHandlerType = typeof(OpLogHandler),
    UseChannelConfigContext = true,
    UseWebSockets = true,
    UseOcr = true,
    UseScalarOpenApi = true,
    InitializeIdWorker = true,
};
builder.AddAgPayApiHost(hostOptions);

// ==================== 以下为 Manager.Api 独有注册 ====================
var services = builder.Services;

services.AddScoped<MchApplymentService>();

// 加入 WebSocket 处理服务
services.AddSingleton<WsChannelUserIdServer>();

// 通知（邮件 / 短信）
services.AddNotice(builder.Configuration);

#region RabbitMQ
// 注册 RabbitMQ 服务并动态注册 Receiver
services.AddRabbitMQServices(
    rabbitMQReceiverTypes: new[]
    {
        typeof(ResetAppConfigRabbitMQReceiver)
    },
    specificReceiverTypes: new[]
    {
        (typeof(ResetAppConfigMQ.IMQReceiver), typeof(ResetAppConfigMQReceiver))
    }
);
#endregion

var app = builder.Build();

// 共享中间件管线
app.UseAgPayApiHost(hostOptions);

app.Run();
