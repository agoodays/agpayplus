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
            // _mqSender.Receive()方法是阻塞的，即在接收消息期间会一直等待，
            // 那么使用Task.Run()和Thread.Sleep()的方式可以在后台启动一个任务，
            // 并通过休眠时间控制消息接收的频率。这种方式可以确保后台任务可以及时响应取消请求，
            // 并且可以调整休眠时间以适应特定的需求。
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000); // 可根据需要调整休眠时间
                }
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // 在停止时执行必要的清理操作
            return Task.CompletedTask;
        }
    }
}
