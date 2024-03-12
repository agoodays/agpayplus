using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk.Models
{
    public class ChannelUserReqModel : AgPayObject
    {
        /// <summary>
        /// 商户号
        /// </summary>
        [JsonProperty("mchNo")]
        public string MchNo { get; set; }

        /// <summary>
        /// 商户应用ID
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// 接口代码,  AUTO表示：自动获取
        /// </summary>
        [JsonProperty("ifCode")]
        public string IfCode { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// 扩展参数，将原样返回
        /// </summary>
        public string ExtParam { get; set; }
    }
}
