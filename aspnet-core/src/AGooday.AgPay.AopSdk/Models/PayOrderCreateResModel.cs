using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 支付下单响应实体类
    /// </summary>
    public class PayOrderCreateResModel : AgPayObject
    {
        /// <summary>
        /// 支付单号(网关生成)
        /// </summary>
        [JsonProperty("payOrderId")]
        public string PayOrderId { get; set; }

        /// <summary>
        /// 商户单号(商户系统生成)
        /// </summary>
        [JsonProperty("mchOrderNo")]
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 订单状态
        /// 0-订单生成
        /// 1-支付中
        /// 2-支付成功
        /// 3-支付失败
        /// 4-已撤销
        /// 5-已退款
        /// 6-订单关闭
        /// </summary>
        [JsonProperty("orderState")]
        public int OrderState { get; set; }

        /// <summary>
        /// 支付参数类型
        /// </summary>
        [JsonProperty("payDataType")]
        public string PayDataType { get; set; }

        /// <summary>
        /// 支付参数
        /// </summary>
        [JsonProperty("payData")]
        public string PayData { get; set; }

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
