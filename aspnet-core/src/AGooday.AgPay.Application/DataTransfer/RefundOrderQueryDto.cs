using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 退款订单表
    /// </summary>
    public class RefundOrderQueryDto : DatePageQuery
    {
        /// <summary>
        /// 退款订单号（支付系统生成订单号）
        /// </summary>
        public string RefundOrderId { get; set; }

        /// <summary>
        /// 三合一订单
        /// </summary>
        public string UnionOrderId { get; set; }

        /// <summary>
        /// 支付订单号（与t_pay_order对应）
        /// </summary>
        public string PayOrderId { get; set; }

        /// <summary>
        /// 渠道支付单号（与t_pay_order channel_order_no对应）
        /// </summary>
        public string ChannelPayOrderNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [BindNever]
        public string MchName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        [BindNever]
        public byte MchType { get; set; }

        /// <summary>
        /// 商户退款单号（商户系统的订单号）
        /// </summary>
        public string MchRefundNo { get; set; }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        public string WayCode { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [BindNever]
        public string IfCode { get; set; }

        /// <summary>
        /// 支付金额,单位分
        /// </summary>
        [BindNever]
        public long PayAmount { get; set; }

        /// <summary>
        /// 退款金额,单位分
        /// </summary>
        [BindNever]
        public long RefundAmount { get; set; }

        /// <summary>
        /// 三位货币代码,人民币:cny
        /// </summary>
        [BindNever]
        public string Currency { get; set; }

        /// <summary>
        /// 退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败,4-退款任务关闭
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [BindNever]
        public string ClientIp { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [BindNever]
        public string RefundReason { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道错误码
        /// </summary>
        [BindNever]
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道错误描述
        /// </summary>
        [BindNever]
        public string ErrMsg { get; set; }

        /// <summary>
        /// 特定渠道发起时额外参数
        /// </summary>
        [BindNever]
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        [BindNever]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [BindNever]
        public string ExtParam { get; set; }

        /// <summary>
        /// 订单退款成功时间
        /// </summary>
        [BindNever]
        public DateTime SuccessTime { get; set; }

        /// <summary>
        /// 退款失效时间（失效后系统更改为退款任务关闭状态）
        /// </summary>
        [BindNever]
        public DateTime ExpiredTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BindNever]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [BindNever]
        public DateTime UpdatedAt { get; set; }
    }
}
