using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Payment.Api.Channel;
using Microsoft.Extensions.Options;
using Pipelines.Sockets.Unofficial;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AGooday.AgPay.Payment.Api.MQ
{
    public class RabbitListener : IHostedService
    {
        private IConnection connection;
        private IModel channel;
        private readonly ILogger<RabbitListener> _logger;
        private readonly IServiceProvider _serviceProvider;
        public RabbitListener(ILogger<RabbitListener> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "127.0.0.1", UserName = "guest", Password = "guest", Port = 5672 };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                var msgReceivers = _serviceProvider.GetServices<IMQMsgReceiver>();
                foreach (var msgReceiver in msgReceivers)
                {
                    var queue = msgReceiver.GetMQName();
                    channel.QueueDeclare(queue: queue,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        msgReceiver.ReceiveMsg(message);

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: queue,
                                         autoAck: false,
                                         consumer: consumer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rabbit连接出现异常");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (connection != null)
                this.connection.Close();
            if (channel != null)
                this.channel.Close();
            return Task.CompletedTask;
        }
    }
}
