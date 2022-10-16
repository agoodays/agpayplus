using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Payment.Api.Channel;
using Microsoft.Extensions.Options;
using Pipelines.Sockets.Unofficial;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AGooday.AgPay.Payment.Api.MQ
{
    public class RabbitListener : IHostedService
    {
        protected readonly IServiceProvider _serviceProvider;
        public RabbitListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        // 注册消费者监听在这里
        private void Register()
        {
            var msgReceivers = _serviceProvider.GetServices<IMQMsgReceiver>();
            foreach (var msgReceiver in msgReceivers)
            {
                msgReceiver.Register();
            }
        }
    }
}
