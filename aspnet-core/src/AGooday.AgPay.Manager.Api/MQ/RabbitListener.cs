using AGooday.AgPay.Components.MQ.Constant;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AGooday.AgPay.Manager.Api.MQ
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
                var factory = new ConnectionFactory()
                {
                    HostName = RabbitMQConfig.MQ.HostName,
                    UserName = RabbitMQConfig.MQ.UserName,
                    Password = RabbitMQConfig.MQ.Password,
                    Port = RabbitMQConfig.MQ.Port
                };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                var msgReceivers = _serviceProvider.GetServices<IMQMsgReceiver>();
                foreach (var msgReceiver in msgReceivers)
                {
                    string queue = string.Empty;
                    if (msgReceiver.GetMQType() == MQSendTypeEnum.QUEUE)
                    {
                        queue = msgReceiver.GetMQName();
                        channel.QueueDeclare(queue, true, false, false, null);
                    }
                    else
                    {
                        var exchange = RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX + msgReceiver.GetMQName();
                        channel.ExchangeDeclare(exchange, "fanout");
                        queue = channel.QueueDeclare().QueueName;
                        channel.QueueBind(queue, exchange, "");
                    }

                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        msgReceiver.ReceiveMsg(message);

                        channel.BasicAck(ea.DeliveryTag, false);
                    };
                    channel.BasicConsume(queue, false, consumer);
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
            if (channel != null)
                this.channel.Close();
            if (connection != null)
                this.connection.Close();
            return Task.CompletedTask;
        }
    }
}
