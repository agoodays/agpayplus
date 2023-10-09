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
            // 启动后台任务，在后台接收消息
            _mqSender.Receive();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // 在停止时执行必要的清理操作
            _mqSender.Close();
            return Task.CompletedTask;
        }
    }
}
