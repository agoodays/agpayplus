using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Constant;
using AGooday.AgPay.Components.MQ.Models;
using RabbitMQ.Client;
using System.Text;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ
{
    public class RabbitMQSender : IMQSender
    {
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

        private static void ConvertAndSend(MQSendTypeEnum mqtype, string exchange, string queue, string routingKey, string message)
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
                LogUtil<RabbitMQSender>.Error("Rabbit连接出现异常", e);
            }
        }

        private static void ConvertAndDelaySend(string exchange, string queue, string routingKey, string message, int delay)
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
                LogUtil<RabbitMQSender>.Error("Rabbit连接出现异常", e);
            }
        }
    }
}
