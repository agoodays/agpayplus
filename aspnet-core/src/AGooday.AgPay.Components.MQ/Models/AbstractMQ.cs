using AGooday.AgPay.Components.MQ.Constant;

namespace AGooday.AgPay.Components.MQ.Models
{
    /// <summary>
    /// 定义MQ消息格式
    /// </summary>
    public abstract class AbstractMQ
    {
        /// <summary>
        /// MQ名称
        /// </summary>
        /// <returns></returns>
        public abstract string GetMQName();

        /// <summary>
        /// MQ 类型
        /// </summary>
        /// <returns></returns>
        public abstract MQSendTypeEnum GetMQType();

        /// <summary>
        /// 构造MQ消息体 String类型
        /// </summary>
        /// <returns></returns>
        public abstract string ToMessage();
    }
}