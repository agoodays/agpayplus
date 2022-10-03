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
    /// 退款下单响应实体类
    /// </summary>
    public class RefundOrderCreateResModel : AgPayObject
    {
        /// <summary>
        /// 退款单号(网关生成)
        /// </summary>
        [JsonProperty("refundOrderId")]
        public string RefundOrderId{get;set;}

        /// <summary>
        /// 商户发起的退款订单号
        /// </summary>
        [JsonProperty("mchRefundNo")]
        public string MchRefundNo{get;set;}

        /// <summary>
        /// 订单支付金额
        /// </summary>
        [JsonProperty("payAmount")]
        public long PayAmount{get;set;}

        /// <summary>
        /// 申请退款金额
        /// </summarY>
        [JsonProperty("refundAmount")]
        public long RefundAmount{get;set;}

        /// <summary>
        /// 退款状态
        /// 0-订单生成
        /// 1-退款中
        /// 2-退款成功
        /// 3-退款失败
        /// 4-退款关闭
        /// </summary>
        [JsonProperty("state")]
        public int State{get;set;}

        /// <summary>
        /// 渠道退款单号
        /// </summary>
        [JsonProperty("channelOrderNo")]
        public string ChannelOrderNo{get;set;}

        /// <summary>
        /// 支付渠道错误码
        /// </summary>
        [JsonProperty("errCode")]
        public string ErrCode{get;set;}

        /// <summary>
        /// 支付渠道错误信息
        /// </summary>
        [JsonProperty("errMsg")]
        public string ErrMsg{get;set;}
    }
}
