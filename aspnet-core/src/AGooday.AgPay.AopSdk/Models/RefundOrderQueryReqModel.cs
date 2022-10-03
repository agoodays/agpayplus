using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 退款查单请求实体类
    /// </summary>
    public class RefundOrderQueryReqModel : AgPayObject
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
        /// 商户退款单号
        /// </summary>
        [JsonProperty("mchRefundNo")]
        public string MchRefundNo { get; set; }
        /// <summary>
        /// 支付系统退款订单号
        /// </summary>
        [JsonProperty("refundOrderId")]
        public string RefundOrderId { get; set; }
    }
}
