using AGooday.AgPay.Components.MQ.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        void ReceiveMsg(string msg);
    }
}
