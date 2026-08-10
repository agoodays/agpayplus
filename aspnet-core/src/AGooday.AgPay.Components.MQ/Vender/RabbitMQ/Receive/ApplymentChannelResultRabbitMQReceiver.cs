using AGooday.AgPay.Components.MQ.Constants;
using AGooday.AgPay.Components.MQ.Models;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive
{
    /// <summary>
    /// RabbitMQ接收器：商户进件渠道结果回写
    /// 
    /// 当业务层实现了 ApplymentChannelResultMQ.IMQReceiver 接口时，
    /// 该类会自动被 DI 容器扫描并实例化，开始监听 QUEUE_APPLYMENT_CHANNEL_RESULT 队列
    /// </summary>
    [RabbitMQReceiver]
    public class ApplymentChannelResultRabbitMQReceiver : IMQMsgReceiver
    {
        private readonly ApplymentChannelResultMQ.IMQReceiver mqReceiver;

        public ApplymentChannelResultRabbitMQReceiver(ApplymentChannelResultMQ.IMQReceiver mqReceiver)
        {
            this.mqReceiver = mqReceiver;
        }

        public MQSendTypeEnum GetMQType() => ApplymentChannelResultMQ.MQ_TYPE;
        public string GetMQName() => ApplymentChannelResultMQ.MQ_NAME;

        public Task ReceiveMsgAsync(string msg)
        {
            return mqReceiver.ReceiveAsync(ApplymentChannelResultMQ.Parse(msg));
        }
    }
}
