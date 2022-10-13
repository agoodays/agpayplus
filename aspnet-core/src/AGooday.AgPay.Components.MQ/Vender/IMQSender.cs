using AGooday.AgPay.Components.MQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Vender
{
    /// <summary>
    /// MQ 消息发送器 接口定义
    /// </summary>
    public interface IMQSender
    {
        /// <summary>
        /// 推送MQ消息， 实时
        /// </summary>
        /// <param name="mqModel"></param>
        void Send(AbstractMQ mqModel);

        /// <summary>
        /// 推送MQ消息， 延迟接收，单位：s
        /// </summary>
        /// <param name="mqModel"></param>
        /// <param name="delay"></param>
        void Send(AbstractMQ mqModel, int delay);
    }
}
