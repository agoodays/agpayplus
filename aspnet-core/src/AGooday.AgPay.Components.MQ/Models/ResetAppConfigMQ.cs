using AGooday.AgPay.Components.MQ.Constant;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.MQ.Models
{
    /// <summary>
    /// 定义MQ消息格式
    /// 业务场景： [ 更新系统配置参数 ]
    /// </summary>
    public class ResetAppConfigMQ : AbstractMQ
    {
        /// <summary>
        /// 【！重要配置项！】 定义MQ名称
        /// </summary>
        public static readonly string MQ_NAME = "BROADCAST_RESET_APP_CONFIG";
        public static readonly MQSendTypeEnum MQ_TYPE = MQSendTypeEnum.BROADCAST;

        /// <summary>
        /// 内置msg 消息体定义
        /// </summary>
        private MsgPayload Payload;

        public ResetAppConfigMQ(MsgPayload payload) => Payload = payload;

        /// <summary>
        /// 【！重要配置项！】 定义Msg消息载体
        /// </summary>
        public class MsgPayload
        {
            public string GroupKey { get; private set; }

            public MsgPayload(string groupKey) => GroupKey = groupKey;
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
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public static ResetAppConfigMQ Build(string groupKey) => new ResetAppConfigMQ(new MsgPayload(groupKey));

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
