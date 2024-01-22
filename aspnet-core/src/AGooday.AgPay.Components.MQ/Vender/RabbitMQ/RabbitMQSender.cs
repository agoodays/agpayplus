using AGooday.AgPay.Components.MQ.Constants;
using AGooday.AgPay.Components.MQ.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ
{
    /// <summary>
    /// https://github.com/whuanle/learnrabbitmq
    /// </summary>
    public class RabbitMQSender : IMQSender
    {
        private IConnection connection;
        private IModel channel;

        private readonly ILogger<RabbitMQSender> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQSender(ILogger<RabbitMQSender> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void Send(AbstractMQ mqModel)
        {
            if (mqModel.GetMQType() == MQSendTypeEnum.QUEUE)
            {
                ConvertAndSend(mqModel.GetMQType(), "", mqModel.GetMQName(), mqModel.GetMQName(), mqModel.ToMessage());
            }
            else
            {
                // fanout模式 的 routeKEY 没意义。
                ConvertAndSend(mqModel.GetMQType(), RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX + mqModel.GetMQName(), "", "", mqModel.ToMessage());
            }
        }

        public void Send(AbstractMQ mqModel, int delay)
        {
            if (mqModel.GetMQType() == MQSendTypeEnum.QUEUE)
            {
                var queue = mqModel.GetMQName();
                ConvertAndDelaySend(RabbitMQConfig.DELAYED_EXCHANGE_NAME, queue, "delay.delay", mqModel.ToMessage(), delay);
            }
            else
            {
                // fanout模式 的 routeKEY 没意义。  没有延迟属性
                ConvertAndSend(mqModel.GetMQType(), RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX + mqModel.GetMQName(), "", "", mqModel.ToMessage());
            }
        }

        public void Receive()
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
                var msgReceivers = _serviceProvider.GetServices<IMQMsgReceiver>()
                    .Where(w => $"{w.GetType().Name}".EndsWith("RabbitMQReceiver", StringComparison.OrdinalIgnoreCase));
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
                //LogUtil<RabbitMQSender>.Error("RabbitMQ消息接收出现异常", ex);
                //throw;
                _logger.LogError(ex, "RabbitMQ消息接收出现异常");
            }
        }

        public void Close()
        {
            if (channel != null)
                this.channel.Close();
            if (connection != null)
                this.connection.Close();
        }

        private void ConvertAndSend(MQSendTypeEnum mqtype, string exchange, string queue, string routingKey, string message)
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
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    if (mqtype == MQSendTypeEnum.QUEUE && !string.IsNullOrWhiteSpace(queue))
                    {
                        channel.QueueDeclare(queue, true, false, false, null);
                    }

                    if (mqtype == MQSendTypeEnum.BROADCAST && !string.IsNullOrWhiteSpace(exchange))
                    {
                        channel.ExchangeDeclare(exchange, type: "fanout");
                    }

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange, routingKey, properties, body);
                }
            }
            catch (Exception e)
            {
                //LogUtil<RabbitMQSender>.Error("RabbitMQ消息推送出现异常", e);
                //throw;
                _logger.LogError(e, "RabbitMQ消息推送出现异常");
            }
        }

        private void ConvertAndDelaySend(string exchange, string queue, string routingKey, string message, int delay)
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
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //设置Exchange队列类型
                    var arguments = new Dictionary<string, object>()
                    {
                        {"x-delayed-type", "topic"}
                    };
                    //设置当前消息为延时队列, 需要安装延时插件: https://www.yuque.com/xiangyisheng/kgcg9t/vmhkyo
                    channel.ExchangeDeclare(exchange, "x-delayed-message", true, false, arguments);
                    channel.QueueDeclare(queue, true, false, false, arguments);
                    channel.QueueBind(queue, exchange, routingKey);

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    //设置消息的过期时间
                    properties.Headers = new Dictionary<string, object>()
                    {
                        {  "x-delay", delay * 1000 }
                    };

                    channel.BasicPublish(exchange, routingKey, properties, body);
                }
            }
            catch (Exception e)
            {
                //LogUtil<RabbitMQSender>.Error("RabbitMQ延迟消息推送出现异常", e);
                //throw;
                _logger.LogError(e, "RabbitMQ延迟消息推送出现异常");
            }
        }
    }
}
