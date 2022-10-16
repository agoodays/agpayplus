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
                ConvertAndSend("",mqModel.GetMQName(), "task_queue", mqModel.ToMessage());
            }
            else
            {
                // fanout模式 的 routeKEY 没意义。
                ConvertAndSend(RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX, mqModel.GetMQName(), "", mqModel.ToMessage());
            }
        }

        public void Send(AbstractMQ mqModel, int delay)
        {
            if (mqModel.GetMQType() == MQSendTypeEnum.QUEUE)
            {
                //rabbitTemplate.convertAndSend(RabbitMQConfig.DELAYED_EXCHANGE_NAME, mqModel.getMQName(), mqModel.toMessage(), messagePostProcessor->{
                //    messagePostProcessor.getMessageProperties().setDelay(Math.toIntExact(delay * 1000));
                //    return messagePostProcessor;
                //});
            }
            else
            {
                // fanout模式 的 routeKEY 没意义。  没有延迟属性
                ConvertAndSend(RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX, mqModel.GetMQName(), "", mqModel.ToMessage());
            }
        }

        private static void ConvertAndSend(string exchange, string queue, string routingKey, string message)
        {
            var factory = new ConnectionFactory() { HostName = "127.0.0.1", UserName = "guest", Password = "guest", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     basicProperties: properties,
                                     body: body);
            }
        }
    }
}
