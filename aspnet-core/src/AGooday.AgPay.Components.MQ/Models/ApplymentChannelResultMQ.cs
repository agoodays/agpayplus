using AGooday.AgPay.Components.MQ.Constants;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.MQ.Models
{
    /// <summary>
    /// MQ消息格式：商户进件渠道结果回写
    ///
    /// 业务场景：
    ///   1. 定时任务（ApplymentReissueJob）轮询渠道返回结果后，不直接写 DB，发 MQ 解耦
    ///   2. 业务编排层（MchApplymentService）调完通道查询/签约等接口后，发 MQ 异步写回
    ///
    /// 消息消费者收到后调用 IMchApplyService.PatchChannelResultAsync 写 DB
    /// </summary>
    public class ApplymentChannelResultMQ : AbstractMQ
    {
        public static readonly string MQ_NAME = "QUEUE_APPLYMENT_CHANNEL_RESULT";
        public static readonly MQSendTypeEnum MQ_TYPE = MQSendTypeEnum.QUEUE;

        private MsgPayload Payload;

        public ApplymentChannelResultMQ(MsgPayload payload) => Payload = payload;

        public class MsgPayload
        {
            /// <summary>进件单号</summary>
            public string ApplyId { get; private set; }

            /// <summary>更新后的状态（null 表示不更新）</summary>
            public byte? State { get; private set; }

            /// <summary>进件进度百分比</summary>
            public byte Progress { get; private set; }

            /// <summary>渠道申请单号</summary>
            public string ChannelApplyNo { get; private set; }

            /// <summary>渠道返回的商户标识</summary>
            public string ChannelMchId { get; private set; }

            /// <summary>申请错误信息</summary>
            public string ApplyErrorInfo { get; private set; }

            /// <summary>成功响应参数（JSON）</summary>
            public string SuccResParameter { get; private set; }

            /// <summary>签约链接</summary>
            public string SignUrl { get; private set; }

            /// <summary>签约链接过期时间</summary>
            public DateTime? SignExpireAt { get; private set; }

            /// <summary>小额打款金额（分）</summary>
            public long? VerifyAmount { get; private set; }

            public MsgPayload(
                string applyId, byte? state, byte progress,
                string channelApplyNo = null, string channelMchId = null,
                string applyErrorInfo = null, string succResParameter = null,
                string signUrl = null, DateTime? signExpireAt = null,
                long? verifyAmount = null)
            {
                ApplyId = applyId;
                State = state;
                Progress = progress;
                ChannelApplyNo = channelApplyNo;
                ChannelMchId = channelMchId;
                ApplyErrorInfo = applyErrorInfo;
                SuccResParameter = succResParameter;
                SignUrl = signUrl;
                SignExpireAt = signExpireAt;
                VerifyAmount = verifyAmount;
            }
        }

        public override string GetMQName() => MQ_NAME;
        public override MQSendTypeEnum GetMQType() => MQ_TYPE;
        public override string ToMessage() => JsonConvert.SerializeObject(Payload);

        public static ApplymentChannelResultMQ Build(MsgPayload payload) => new ApplymentChannelResultMQ(payload);
        public static MsgPayload Parse(string msg) => JsonConvert.DeserializeObject<MsgPayload>(msg);

        /// <summary>
        /// IMQReceiver 接口 — 业务层实现此接口即可自动接收到消息
        /// </summary>
        public interface IMQReceiver
        {
            Task ReceiveAsync(MsgPayload payload);
        }
    }
}
