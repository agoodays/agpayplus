using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 退款查单响应实体类
    /// </summary>
    public class RefundOrderQueryResModel : AgPayObject
    {
        /// <summary>
        /// 退款订单号（支付系统生成订单号）
        /// </summary>
        [JsonProperty("refundOrderId")]
        public string RefundOrderId { get; set; }

        /// <summary>
        /// 支付订单号
        /// </summary>
        [JsonProperty("payOrderId")]
        public string PayOrderId { get; set; }

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
        /// 支付金额,单位分
        /// </summary>
        [JsonProperty("payAmount")]
        public long PayAmount { get; set; }

        /// <summary>
        /// 退款金额,单位分
        /// </summary>
        [JsonProperty("refundAmount")]
        public long RefundAmount { get; set; }

        /// <summary>
        /// 三位货币代码,人民币:cny
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 退款状态
        /// 0-订单生成
        /// 1-退款中
        /// 2-退款成功
        /// 3-退款失败
        /// 4-退款关闭
        /// </summary>
        [JsonProperty("state")]
        public byte State { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        [JsonProperty("channelOrderNo")]
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道错误码
        /// </summary>
        [JsonProperty("errCode")]
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道错误描述
        /// </summary>
        [JsonProperty("errMsg")]
        public string ErrMsg { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [JsonProperty("extParam")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 订单创建时间,13位时间戳
        /// </summary>
        [JsonProperty("createdAt")]
        public long CreatedAt { get; set; }

        /// <summary>
        /// 订单支付成功时间,13位时间戳
        /// </summary>
        [JsonProperty("successTime")]
        public long SuccessTime { get; set; }
    }
}
