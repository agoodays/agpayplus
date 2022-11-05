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
        public static readonly string MQ_VENDER_KEY = "MQ.Vender";

        public static readonly string ACTIVE_MQ = "ActiveMQ";
        public static readonly string RABBIT_MQ = "RabbitMQ";
        public static readonly string ROCKET_MQ = "RocketMQ";
        public static readonly string ALIYUN_ROCKET_MQ = "aliYunRocketMQ";
    }
}
