using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Constant
{
    /// <summary>
    /// 定义MQ消息类型
    /// </summary>
    public enum MQSendTypeEnum
    {
        /** QUEUE - 点对点 （只有1个消费者可消费。 ActiveMQ的queue模式 ） **/
        QUEUE,
        /** BROADCAST - 订阅模式 (所有接收者都可接收到。 ActiveMQ的topic模式, RabbitMQ的fanout类型的交换机, RocketMQ的广播模式  ) **/
        BROADCAST
    }
}
