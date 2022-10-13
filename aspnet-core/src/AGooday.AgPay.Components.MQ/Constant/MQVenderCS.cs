using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Constant
{
    /// <summary>
    /// MQ 厂商定义类
    /// </summary>
    public class MQVenderCS
    {
        public static readonly string YML_VENDER_KEY = "isys.mq.vender";

        public static readonly string ACTIVE_MQ = "activeMQ";
        public static readonly string RABBIT_MQ = "rabbitMQ";
        public static readonly string ROCKET_MQ = "rocketMQ";
        public static readonly string ALIYUN_ROCKET_MQ = "aliYunRocketMQ";
    }
}
