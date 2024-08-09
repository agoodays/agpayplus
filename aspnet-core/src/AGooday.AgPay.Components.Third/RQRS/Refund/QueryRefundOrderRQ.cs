namespace AGooday.AgPay.Components.Third.RQRS.Refund
{
    /// <summary>
    /// 查询退款单请求参数对象
    /// </summary>
    public class QueryRefundOrderRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 商户退款单号
        /// </summary>
        public string MchRefundNo { get; set; }

        /// <summary>
        /// 支付系统退款订单号
        /// </summary>
        public string RefundOrderId { get; set; }
    }
}
