namespace AGooday.AgPay.Components.MQ.Constants
{
    /// <summary>
    /// 定义MQ消息类型
    /// </summary>
    public enum MQSendTypeEnum
    {
        /// <summary>
        /// QUEUE - 点对点 （只有1个消费者可消费。 ActiveMQ的queue模式 ）
        /// </summary>
        QUEUE,
        /// <summary>
        /// BROADCAST - 订阅模式 (所有接收者都可接收到。 ActiveMQ的topic模式, RabbitMQ的fanout类型的交换机, RocketMQ的广播模式  )
        /// </summary>
        BROADCAST
    }
}
