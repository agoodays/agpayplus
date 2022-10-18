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
                ConvertAndSend("", mqModel.GetMQName(), "", mqModel.ToMessage());
            }
            else
            {
                // fanout模式 的 routeKEY 没意义。
                ConvertAndSend(RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX + mqModel.GetMQName(), "", "", mqModel.ToMessage());
            }
        }

        public void Send(AbstractMQ mqModel, int delay)
        {
            if (mqModel.GetMQType() == MQSendTypeEnum.QUEUE)
            {
                var queue = mqModel.GetMQName();
                //https://jaskey.github.io/blog/2018/08/15/rabbitmq-delay-queue/
                Dictionary<string, object> arguments = new Dictionary<string, object>();
                arguments.Add("x-expires", delay * 60 * 1000);
                arguments.Add("x-message-ttl", delay * 1000);//队列上消息过期时间，应小于队列过期时间  
                arguments.Add("x-dead-letter-exchange", "");//过期消息转向指定的exchange中
                arguments.Add("x-dead-letter-routing-key", queue);//过期消息转向路由相匹配routingkey 
                ConvertAndSend(RabbitMQConfig.DELAYED_EXCHANGE_NAME, queue, "", mqModel.ToMessage(), arguments);
            }
            else
            {
                // fanout模式 的 routeKEY 没意义。  没有延迟属性
                ConvertAndSend(RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX + mqModel.GetMQName(), "", "", mqModel.ToMessage());
            }
        }

        private static void ConvertAndSend(string exchange, string queue, string routingKey, string message, Dictionary<string, object> arguments = null)
        {
            var hostName = Appsettings.app(new string[] { "MQ", "RabbitMQ", "HostName" });
            var userName = Appsettings.app(new string[] { "MQ", "RabbitMQ", "UserName" });
            var password = Appsettings.app(new string[] { "MQ", "RabbitMQ", "Password" });
            var port = Convert.ToInt32(Appsettings.app(new string[] { "MQ", "RabbitMQ", "Port" }));
            var factory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password, Port = port };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: arguments);

                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: exchange,
                                     routingKey: queue,
                                     basicProperties: properties,
                                     body: body);
            }
        }
    }
}
