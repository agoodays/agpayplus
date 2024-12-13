using AGooday.AgPay.Components.MQ.Constants;

namespace AGooday.AgPay.Components.MQ.Vender
{
    /// <summary>
    /// MQ 消息接收器 接口定义
    /// </summary>
    public interface IMQMsgReceiver
    {
        /// <summary>
        /// MQ类型
        /// </summary>
        /// <returns></returns>
        MQSendTypeEnum GetMQType();

        /// <summary>
        /// MQ名称
        /// </summary>
        /// <returns></returns>
        string GetMQName();

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="msg"></param>
        Task ReceiveMsgAsync(string msg);
    }
}
