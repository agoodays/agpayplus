using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Constant;
using AGooday.AgPay.Components.MQ.Models;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var hostName = Appsettings.app(new string[] { "MQ", "RabbitMQ", "HostName" });
            var userName = Appsettings.app(new string[] { "MQ", "RabbitMQ", "UserName" });
            var password = Appsettings.app(new string[] { "MQ", "RabbitMQ", "Password" });
            var port = Convert.ToInt32(Appsettings.app(new string[] { "MQ", "RabbitMQ", "Port" }));
            var factory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password, Port = port };
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

        private static void ConvertAndDelaySend(string exchange, string queue, string routingKey, string message, int delay)
        {
            var hostName = Appsettings.app(new string[] { "MQ", "RabbitMQ", "HostName" });
            var userName = Appsettings.app(new string[] { "MQ", "RabbitMQ", "UserName" });
            var password = Appsettings.app(new string[] { "MQ", "RabbitMQ", "Password" });
            var port = Convert.ToInt32(Appsettings.app(new string[] { "MQ", "RabbitMQ", "Port" }));
            var factory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password, Port = port };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //设置Exchange队列类型
                var arguments = new Dictionary<string, object>()
                {
                    {"x-delayed-type", "topic"}
                };
                //设置当前消息为延时队列
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
    }
}
