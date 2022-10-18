using AGooday.AgPay.Components.MQ.Constant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Models
{
    /// <summary>
    /// 定义MQ消息格式
    /// 业务场景： [ 支付订单补单（一般用于没有回调的接口，比如微信的条码支付） ]
    /// </summary>
    public class PayOrderReissueMQ : AbstractMQ
    {
        /// <summary>
        /// 【！重要配置项！】 定义MQ名称
        /// </summary>
        public static readonly string MQ_NAME = "QUEUE_PAY_ORDER_REISSUE";
        public static readonly MQSendTypeEnum MQ_TYPE = MQSendTypeEnum.QUEUE;

        /// <summary>
        /// 内置msg 消息体定义
        /// </summary>
        private MsgPayload Payload;

        public PayOrderReissueMQ(MsgPayload payload) => Payload = payload;

        /// <summary>
        /// 【！重要配置项！】 定义Msg消息载体
        /// </summary>
        public class MsgPayload
        {
            /// <summary>
            /// 支付订单号
            /// </summary>
            public string PayOrderId { get; private set; }

            /// <summary>
            /// 通知次数
            /// </summary>
            public int Count { get; private set; }

            public MsgPayload(string payOrderId, int count)
            {
                PayOrderId = payOrderId;
                Count = count;
            }
        }

        public override string GetMQName() => MQ_NAME;

        /// <summary>
        /// 【！重要配置项！】
        /// </summary>
        /// <returns></returns>
        public override MQSendTypeEnum GetMQType() => MQ_TYPE;  // QUEUE - 点对点 、 BROADCAST - 广播模式

        public override string ToMessage() => JsonConvert.SerializeObject(Payload);

        /// <summary>
        /// 【！重要配置项！】 构造MQModel , 一般用于发送MQ时
        /// </summary>
        /// <returns></returns>
        public static PayOrderReissueMQ Build(string payOrderId, int count) => new PayOrderReissueMQ(new MsgPayload(payOrderId, count));

        /// <summary>
        /// 解析MQ消息， 一般用于接收MQ消息时
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static MsgPayload Parse(string msg) => JsonConvert.DeserializeObject<MsgPayload>(msg);

        /// <summary>
        /// 定义 IMQReceiver 接口： 项目实现该接口则可接收到对应的业务消息
        /// </summary>
        public interface IMQReceiver
        {
            void Receive(MsgPayload payload);
        }
    }
}
