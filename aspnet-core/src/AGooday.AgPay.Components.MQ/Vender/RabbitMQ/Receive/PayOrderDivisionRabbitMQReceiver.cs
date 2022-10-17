using AGooday.AgPay.Components.MQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive
{
    /// <summary>
    /// rabbitMQ消息接收器：仅在vender=rabbitMQ时 && 项目实现IMQReceiver接口时 进行实例化
    /// 业务：  支付订单分账通知
    /// </summary>
    public class PayOrderDivisionRabbitMQReceiver : IMQMsgReceiver
    {
        private PayOrderDivisionMQ.IMQReceiver mqReceiver;

        public PayOrderDivisionRabbitMQReceiver(PayOrderDivisionMQ.IMQReceiver mqReceiver)
        {
            this.mqReceiver = mqReceiver;
        }

        public string GetMQName() => PayOrderDivisionMQ.MQ_NAME;

        /// <summary>
        /// 接收 【 queue 】 类型的消息
        /// </summary>
        /// <param name="msg"></param>
        public void ReceiveMsg(string msg)
        {
            mqReceiver.Receive(PayOrderDivisionMQ.Parse(msg));
        }
    }
}
