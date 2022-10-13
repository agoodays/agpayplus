using AGooday.AgPay.Components.MQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive
{
    /// <summary>
    /// rabbitMQ消息接收器：仅在vender=rabbitMQ时 && 项目实现IMQReceiver接口时 进行实例化
    /// 业务：  支付订单商户通知
    /// </summary>
    public class PayOrderMchNotifyRabbitMQReceiver : IMQMsgReceiver
    {
        private PayOrderMchNotifyMQ.IMQReceiver mqReceiver;

        public PayOrderMchNotifyRabbitMQReceiver(PayOrderMchNotifyMQ.IMQReceiver mqReceiver)
        {
            this.mqReceiver = mqReceiver;
        }

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
