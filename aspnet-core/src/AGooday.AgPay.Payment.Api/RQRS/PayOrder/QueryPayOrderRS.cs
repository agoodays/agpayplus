using AGooday.AgPay.Application.DataTransfer;
using AutoMapper;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    /// <summary>
    /// 查询订单 响应参数
    /// </summary>
    public class QueryPayOrderRS : AbstractRS
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string PayOrderId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 商户应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        public string IfCode { get; set; }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        public string WayCode { get; set; }

        /// <summary>
        /// 支付金额,单位分
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// 三位货币代码,人民币:cny
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 支付状态: 0-订单生成, 1-支付中, 2-支付成功, 3-支付失败, 4-已撤销, 5-已退款, 6-订单关闭
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 商品描述信息
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道支付错误码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道支付错误描述
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        public string ExtParam { get; set; }

        /// <summary>
        /// 订单支付成功时间
        /// </summary>
        public long? SuccessTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long? CreatedAt { get; set; }

        public static QueryPayOrderRS BuildByPayOrder(PayOrderDto payOrder)
        {
            if (payOrder == null)
            {
                return null;
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<PayOrderDto, QueryPayOrderRS>());
            var mapper = config.CreateMapper();
            var result = mapper.Map<PayOrderDto, QueryPayOrderRS>(payOrder);
            result.SuccessTime = payOrder.SuccessTime == null ? null : new DateTimeOffset(payOrder.SuccessTime.Value).ToUnixTimeSeconds();
            result.CreatedAt = payOrder.CreatedAt == null ? null : new DateTimeOffset(payOrder.CreatedAt.Value).ToUnixTimeSeconds();

            return result;
        }
    }
}
