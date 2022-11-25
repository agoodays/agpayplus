using AGooday.AgPay.Components.MQ.Constant;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.MQ.Models
{
    /// <summary>
    /// 定义MQ消息格式
    /// 业务场景： [ 清除商户登录信息 ]
    /// </summary>
    public class CleanMchLoginAuthCacheMQ : AbstractMQ
    {
        /// <summary>
        /// 【！重要配置项！】 定义MQ名称
        /// </summary>
        public static readonly string MQ_NAME = "QUEUE_CLEAN_MCH_LOGIN_AUTH_CACHE";
        public static readonly MQSendTypeEnum MQ_TYPE = MQSendTypeEnum.QUEUE;

        /// <summary>
        /// 内置msg 消息体定义
        /// </summary>
        private MsgPayload Payload;

        public CleanMchLoginAuthCacheMQ(MsgPayload payload) => Payload = payload;

        /// <summary>
        /// 【！重要配置项！】 定义Msg消息载体
        /// </summary>
        public class MsgPayload
        {
            /// <summary>
            /// 用户ID集合
            /// </summary>
            public List<long> UserIdList { get; private set; }

            public MsgPayload(List<long> userIdList) => UserIdList = userIdList;
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
        /// <param name="userIdList"></param>
        /// <returns></returns>
        public static CleanMchLoginAuthCacheMQ Build(List<long> userIdList) => new CleanMchLoginAuthCacheMQ(new MsgPayload(userIdList));

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
