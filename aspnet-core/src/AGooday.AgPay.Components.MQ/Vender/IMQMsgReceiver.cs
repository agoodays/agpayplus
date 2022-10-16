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
        /// 注册消费者
        /// </summary>
        void Register();

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="msg"></param>
        void ReceiveMsg(string msg);
    }
}
