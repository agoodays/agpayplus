using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 转账查单请求实体类
    /// </summary>
    public class TransferOrderQueryReqModel : AgPayObject
    {
        /// <summary>
        /// 转账订单号
        /// </summary>
        [JsonProperty("transferId")]
        public string TransferId { get; set; }

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
    }
}
