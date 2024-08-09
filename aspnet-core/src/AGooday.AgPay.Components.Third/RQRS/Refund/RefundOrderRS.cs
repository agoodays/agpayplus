using AGooday.AgPay.Application.DataTransfer;
using AutoMapper;

namespace AGooday.AgPay.Components.Third.RQRS.Refund
{
    /// <summary>
    /// 退款订单 响应参数
    /// </summary>
    public class RefundOrderRS : AbstractRS
    {
        /// <summary>
        /// 支付系统退款订单号
        /// </summary>
        public string RefundOrderId { get; set; }

        /// <summary>
        /// 商户发起的退款订单号
        /// </summary>
        public string MchRefundNo { get; set; }

        /// <summary>
        /// 订单支付金额
        /// </summary>
        public long PayAmount { get; set; }

        /// <summary>
        /// 申请退款金额
        /// </summary>
        public long RefundAmount { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 渠道退款单号
        /// </summary>
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道返回错误代码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道返回错误信息
        /// </summary>
        public string ErrMsg { get; set; }

        public static RefundOrderRS BuildByRefundOrder(RefundOrderDto refundOrder)
        {
            if (refundOrder == null)
            {
                return null;
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<RefundOrderDto, RefundOrderRS>());
            var mapper = config.CreateMapper();
            var result = mapper.Map<RefundOrderDto, RefundOrderRS>(refundOrder);

            return result;
        }
    }
}
