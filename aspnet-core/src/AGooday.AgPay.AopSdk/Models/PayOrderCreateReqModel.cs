using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 支付下单请求实体类
    /// </summary>
    public class PayOrderCreateReqModel : AgPayObject
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
        /// 支付方式
        /// </summary>
        [JsonProperty("wayCode")]
        public string WayCode { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [JsonProperty("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// 货币代码，当前只支持cny
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [JsonProperty("clientIp")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        [JsonProperty("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// 卖家备注
        /// </summary>
        [JsonProperty("sellerRemark")]
        public string SellerRemark { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
        [JsonProperty("buyerRemark")]
        public string BuyerRemark { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [JsonProperty("storeId")]
        public long? StoreId { get; set; }

        /// <summary>
        /// 二维码ID
        /// </summary>
        [JsonProperty("qrcId")]
        public string QrcId { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        [JsonProperty("notifyUrl")]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 跳转通知地址
        /// </summary>
        [JsonProperty("returnUrl")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 订单失效时间
        /// </summary>
        [JsonProperty("expiredTime")]
        public string ExpiredTime { get; set; }

        /// <summary>
        /// 特定渠道额外支付参数
        /// </summary>
        [JsonProperty("channelExtra")]
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 渠道用户标识,如微信openId,支付宝账号
        /// </summary>
        [JsonProperty("channelUser")]
        public string ChannelUser { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        [JsonProperty("extParam")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 分账模式： 0-该笔订单不允许分账[默认], 1-支付成功按配置自动完成分账, 2-商户手动分账(解冻商户金额)
        /// </summary>
        [JsonProperty("divisionMode")]
        public byte DivisionMode { get; set; }
    }
}
