using AGooday.AgPay.Components.MQ.Vender;

namespace AGooday.AgPay.Agent.Api.MQ
{
    public class MQReceiverHostedService : IHostedService
    {
        private readonly ILogger<MQReceiverHostedService> _logger;
        private readonly IMQSender _mqSender;

        public MQReceiverHostedService(ILogger<MQReceiverHostedService> logger, IMQSender mqSender)
        {
            _logger = logger;
            _mqSender = mqSender;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("启动MQ消息接收服务...");
            // 启动后台任务，在后台接收消息
            return _mqSender.ReceiveAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("停止MQ消息接收服务...");
            // 在停止时执行必要的清理操作
            return _mqSender.CloseAsync();
        }
    }
}
