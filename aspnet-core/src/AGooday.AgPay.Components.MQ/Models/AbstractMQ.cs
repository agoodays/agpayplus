using AGooday.AgPay.Components.MQ.Constant;

namespace AGooday.AgPay.Components.MQ.Models
{
    /// <summary>
    /// 定义MQ消息格式
    /// </summary>
    public abstract class AbstractMQ
    {
        /** MQ名称 **/
        public abstract string GetMQName();

        /** MQ 类型 **/
        public abstract MQSendTypeEnum GetMQType();

        /** 构造MQ消息体 String类型 **/
        public abstract string ToMessage();
    }
}