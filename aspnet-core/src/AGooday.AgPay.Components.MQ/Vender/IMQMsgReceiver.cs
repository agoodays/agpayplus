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
        /** 接收消息 **/
        void ReceiveMsg(string msg);
    }
}
