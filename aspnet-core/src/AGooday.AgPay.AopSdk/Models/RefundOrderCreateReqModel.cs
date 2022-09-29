using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Models
{
    public class RefundOrderCreateReqModel : AgPayObject
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
        /// 支付系统订单号
        /// </summary>
        [JsonProperty("payOrderId")]
        public string PayOrderId { get; set; }
        /// <summary>
        /// 退款单号
        /// </summary>
        [JsonProperty("mchRefundNo")]
        public string MchRefundNo { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [JsonProperty("refundAmount")]
        public long RefundAmount { get; set; }
        /// <summary>
        /// 货币代码，当前只支持cny
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        [JsonProperty("refundReason")]
        public string RefundReason { get; set; }
        /// <summary>
        /// 客户端IP
        /// </summary>
        [JsonProperty("clientIp")]
        public string ClientIp { get; set; }
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
