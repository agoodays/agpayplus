using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 分账账号的响应结果
    /// </summary>
    public class DivisionReceiverBindResModel : AgPayObject
    {
        /// <summary>
        /// 分账接收者ID
        /// </summary>
        [JsonProperty("receiverId")]
        public long ReceiverId { get; set; }

        /// <summary>
        /// 接收者账号别名
        /// </summary>
        [JsonProperty("receiverAlias")]
        public string ReceiverAlias { get; set; }

        /// <summary>
        /// 组ID（便于商户接口使用）
        /// </summary>
        [JsonProperty("receiverId")]
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [JsonProperty("mchNo")]
        public string MchNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [JsonProperty("ifCode")]
        public string IfCode { get; set; }

        /// <summary>
        /// 分账接收账号类型: 0-个人(对私) 1-商户(对公)
        /// </summary>
        [JsonProperty("accType")]
        public byte AccType { get; set; }

        /// <summary>
        /// 分账接收账号
        /// </summary>
        [JsonProperty("accNo")]
        public string AccNo { get; set; }

        /// <summary>
        /// 分账接收账号名称
        /// </summary>
        [JsonProperty("accName")]
        public string AccName { get; set; }

        /// <summary>
        /// 分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等
        /// </summary>
        [JsonProperty("relationType")]
        public string RelationType { get; set; }

        /// <summary>
        /// 当选择自定义时，需要录入该字段。 否则为对应的名称
        /// </summary>
        [JsonProperty("relationTypeName")]
        public string RelationTypeName { get; set; }

        /// <summary>
        /// 渠道特殊信息
        /// </summary>
        [JsonProperty("channelExtInfo")]
        public string ChannelExtInfo { get; set; }

        /// <summary>
        /// 绑定成功时间
        /// </summary>
        [JsonProperty("bindSuccessTime")]
        public string BindSuccessTime { get; set; }

        /// <summary>
        /// 分账比例
        /// </summary>
        [JsonProperty("divisionProfit")]
        public string DivisionProfit { get; set; }

        /// <summary>
        /// 分账状态 1-绑定成功, 0-绑定异常
        /// </summary>
        [JsonProperty("bindState")]
        private byte BindState { get; set; }

        /// <summary>
        /// 支付渠道错误码
        /// </summary>
        [JsonProperty("errCode")]
        public string ErrCode { get; set; }

        /// <summary>
        /// 支付渠道错误信息
        /// </summary>
        [JsonProperty("errMsg")]
        public string ErrMsg { get; set; }
    }
}
