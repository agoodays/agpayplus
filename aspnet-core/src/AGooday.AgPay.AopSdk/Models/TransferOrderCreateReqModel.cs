using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 转账下单请求实体类
    /// </summary>
    public class TransferOrderCreateReqModel : AgPayObject
    {
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
        /// 商户订单号
        /// </summary>
        [JsonProperty("mchOrderNo")]
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [JsonProperty("ifCode")]
        public string IfCode { get; set; }

        /// <summary>
        /// 入账方式
        /// </summary>
        [JsonProperty("entryType")]
        public string EntryType { get; set; }

        /// <summary>
        /// 转账金额
        /// </summary>
        [JsonProperty("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// 货币代码，当前只支持cny
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        [JsonProperty("accountNo")]
        public string AccountNo { get; set; }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 收款人开户行名称
        /// </summary>
        [JsonProperty("bankName")]
        public string BankName { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [JsonProperty("clientIp")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 转账备注信息
        /// </summary>
        [JsonProperty("transferDesc")]
        public string TransferDesc { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        [JsonProperty("notifyUrl")]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 特定渠道额外支付参数
        /// </summary>
        [JsonProperty("channelExtra")]
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        [JsonProperty("extParam")]
        public string ExtParam { get; set; }
    }
}
