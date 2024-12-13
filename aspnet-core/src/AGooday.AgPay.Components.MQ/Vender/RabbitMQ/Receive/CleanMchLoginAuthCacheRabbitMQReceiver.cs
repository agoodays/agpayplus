using AGooday.AgPay.Components.MQ.Constants;
using AGooday.AgPay.Components.MQ.Models;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive
{
    /// <summary>
    /// rabbitMQ消息接收器：仅在vender=rabbitMQ时 && 项目实现IMQReceiver接口时 进行实例化
    /// 业务：支付订单分账通知
    /// </summary>
    public class CleanMchLoginAuthCacheRabbitMQReceiver : IMQMsgReceiver
    {
        private CleanMchLoginAuthCacheMQ.IMQReceiver mqReceiver;

        public CleanMchLoginAuthCacheRabbitMQReceiver(CleanMchLoginAuthCacheMQ.IMQReceiver mqReceiver)
        {
            this.mqReceiver = mqReceiver;
        }

        public MQSendTypeEnum GetMQType() => CleanMchLoginAuthCacheMQ.MQ_TYPE;

        public string GetMQName() => CleanMchLoginAuthCacheMQ.MQ_NAME;

        /// <summary>
        /// 接收 【 queue 】 类型的消息
        /// </summary>
        /// <param name="msg"></param>
        public Task ReceiveMsgAsync(string msg)
        {
            return mqReceiver.ReceiveAsync(CleanMchLoginAuthCacheMQ.Parse(msg));
        }
    }
}
