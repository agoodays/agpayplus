using AGooday.AgPay.Components.MQ.Constant;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.MQ.Models
{
    /// <summary>
    /// 定义MQ消息格式
    /// 业务场景： [ 支付订单补单（一般用于没有回调的接口，比如微信的条码支付） ]
    /// </summary>
    public class ResetIsvMchAppInfoConfigMQ : AbstractMQ
    {
        /// <summary>
        /// 【！重要配置项！】 定义MQ名称
        /// </summary>
        public static readonly string MQ_NAME = "BROADCAST_RESET_ISV_MCH_APP_INFO_CONFIG";
        public static readonly MQSendTypeEnum MQ_TYPE = MQSendTypeEnum.BROADCAST;

        // 重置类型 （枚举类型，无法json反序列化）
        public static readonly byte RESET_TYPE_ISV_INFO = 1;
        public static readonly byte RESET_TYPE_MCH_INFO = 2;
        public static readonly byte RESET_TYPE_MCH_APP = 3;

        /// <summary>
        /// 内置msg 消息体定义
        /// </summary>
        private MsgPayload Payload;

        public ResetIsvMchAppInfoConfigMQ(MsgPayload payload) => Payload = payload;

        /// <summary>
        /// 【！重要配置项！】 定义Msg消息载体
        /// </summary>
        public class MsgPayload
        {
            /// <summary>
            /// 重置类型
            /// </summary>
            public byte ResetType { get; private set; }
            public string IsvNo { get; private set; }
            public string MchNo { get; private set; }
            public string AppId { get; private set; }

            public MsgPayload(byte resetType, string isvNo, string mchNo, string appId)
            {
                ResetType = resetType;
                IsvNo = isvNo;
                MchNo = mchNo;
                AppId = appId;
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
        public static ResetIsvMchAppInfoConfigMQ Build(byte resetType, string isvNo, string mchNo, string appId)
            => new ResetIsvMchAppInfoConfigMQ(new MsgPayload(resetType, isvNo, mchNo, appId));

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
