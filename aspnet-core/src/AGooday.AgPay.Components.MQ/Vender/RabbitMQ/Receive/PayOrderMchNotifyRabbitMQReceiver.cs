using AGooday.AgPay.Components.MQ.Constants;
using AGooday.AgPay.Components.MQ.Models;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive
{
    /// <summary>
    /// rabbitMQ消息接收器：仅在vender=rabbitMQ时 && 项目实现IMQReceiver接口时 进行实例化
    /// 业务：支付订单商户通知
    /// </summary>
    public class PayOrderMchNotifyRabbitMQReceiver : IMQMsgReceiver
    {
        private PayOrderMchNotifyMQ.IMQReceiver mqReceiver;

        public PayOrderMchNotifyRabbitMQReceiver(PayOrderMchNotifyMQ.IMQReceiver mqReceiver)
        {
            this.mqReceiver = mqReceiver;
        }

        public MQSendTypeEnum GetMQType() => PayOrderMchNotifyMQ.MQ_TYPE;

        public string GetMQName() => PayOrderMchNotifyMQ.MQ_NAME;

        /// <summary>
        /// 接收 【 queue 】 类型的消息
        /// </summary>
        /// <param name="msg"></param>
        public void ReceiveMsg(string msg)
        {
            mqReceiver.Receive(PayOrderMchNotifyMQ.Parse(msg));
        }
    }
}
