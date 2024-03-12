using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 关闭订单响应结果
    /// </summary>
    public class PayOrderCloseResModel : AgPayObject
    {
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
