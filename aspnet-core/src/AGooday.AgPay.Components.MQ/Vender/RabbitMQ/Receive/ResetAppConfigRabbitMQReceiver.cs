using AGooday.AgPay.Components.MQ.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive
{
    /// <summary>
    /// rabbitMQ消息接收器：仅在vender=rabbitMQ时 && 项目实现IMQReceiver接口时 进行实例化
    /// 业务：  更新系统配置参数
    /// </summary>
    public class ResetAppConfigRabbitMQReceiver : IMQMsgReceiver
    {
        private ResetAppConfigMQ.IMQReceiver mqReceiver;

        public ResetAppConfigRabbitMQReceiver(ResetAppConfigMQ.IMQReceiver mqReceiver)
        {
            this.mqReceiver = mqReceiver;
        }
        public string GetMQName() => ResetAppConfigMQ.MQ_NAME;

        /// <summary>
        /// 接收 【 queue 】 类型的消息
        /// </summary>
        /// <param name="msg"></param>
        public void ReceiveMsg(string msg)
        {
            mqReceiver.Receive(ResetAppConfigMQ.Parse(msg));
        }
    }
}
