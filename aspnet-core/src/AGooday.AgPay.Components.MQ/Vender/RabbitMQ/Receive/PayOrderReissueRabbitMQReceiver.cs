using AGooday.AgPay.Components.MQ.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGooday.AgPay.Components.MQ.Constant;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive
{
    /// <summary>
    /// rabbitMQ消息接收器：仅在vender=rabbitMQ时 && 项目实现IMQReceiver接口时 进行实例化
    /// 业务：  支付订单补单（一般用于没有回调的接口，比如微信的条码支付）
    /// </summary>
    public class PayOrderReissueRabbitMQReceiver : IMQMsgReceiver
    {
        private PayOrderReissueMQ.IMQReceiver mqReceiver;

        public PayOrderReissueRabbitMQReceiver(PayOrderReissueMQ.IMQReceiver mqReceiver)
        {
            this.mqReceiver = mqReceiver;
        }

        public MQSendTypeEnum GetMQType() => PayOrderReissueMQ.MQ_TYPE;

        public string GetMQName() => PayOrderReissueMQ.MQ_NAME;

        /// <summary>
        /// 接收 【 queue 】 类型的消息
        /// </summary>
        /// <param name="msg"></param>
        public void ReceiveMsg(string msg)
        {
            mqReceiver.Receive(PayOrderReissueMQ.Parse(msg));
        }
    }
}
