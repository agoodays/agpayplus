using AGooday.AgPay.Domain.Core.Events;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Payment.Api.RQRS.Refund
{
    public class RefundOrderRQ : AbstractMchAppRQ
    {

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付系统订单号
        /// </summary>
        public string PayOrderId { get; set; }

        /// <summary>
        /// 商户系统生成的退款单号
        /// </summary>
        [Required(ErrorMessage = "商户退款单号不能为空")]
        public string MchRefundNo { get; set; }

        /// <summary>
        /// 退款金额， 单位：分 
        /// </summary>
        [Required(ErrorMessage = "退款金额不能为空")]
        [Range(1, long.MaxValue, ErrorMessage = "退款金额请大于1分")]
        public long RefundAmount { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        [Required(ErrorMessage = "货币代码不能为空")]
        public string Currency { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [Required(ErrorMessage = "退款原因不能为空")]
        public string RefundReason { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        public string ExtParam { get; set; }
    }
}
