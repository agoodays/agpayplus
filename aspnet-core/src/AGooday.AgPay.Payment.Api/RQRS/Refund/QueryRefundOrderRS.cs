using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AutoMapper;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Payment.Api.RQRS.Refund
{
    /// <summary>
    /// 查询退款单 响应参数
    /// </summary>
    public class QueryRefundOrderRS : AbstractRS
    {
        /// <summary>
        /// 退款订单号（支付系统生成订单号）
        /// </summary>
        public string RefundOrderId { get; set; }

        /// <summary>
        /// 支付订单号（与t_pay_order对应）
        /// </summary>
        public string PayOrderId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户退款单号（商户系统的订单号）
        /// </summary>
        public string MchRefundNo { get; set; }

        /// <summary>
        /// 支付金额,单位分
        /// </summary>
        public long PayAmount { get; set; }

        /// <summary>
        /// 退款金额,单位分
        /// </summary>
        public long RefundAmount { get; set; }

        /// <summary>
        /// 三位货币代码,人民币:cny
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道错误码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道错误描述
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        public string ExtParam { get; set; }

        /// <summary>
        /// 订单退款成功时间
        /// </summary>
        public long? SuccessTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long? CreatedAt { get; set; }

        public static QueryRefundOrderRS BuildByRefundOrder(RefundOrderDto refundOrder)
        {
            if (refundOrder == null)
            {
                return null;
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<RefundOrderDto, QueryRefundOrderRS>());
            var mapper = config.CreateMapper();
            var result = mapper.Map<RefundOrderDto, QueryRefundOrderRS>(refundOrder);
            result.SuccessTime = refundOrder.SuccessTime == null ? null : new DateTimeOffset(refundOrder.SuccessTime.Value).ToUnixTimeSeconds();
            result.CreatedAt = refundOrder.CreatedAt == null ? null : new DateTimeOffset(refundOrder.CreatedAt.Value).ToUnixTimeSeconds();
            return result;
        }
    }
}
