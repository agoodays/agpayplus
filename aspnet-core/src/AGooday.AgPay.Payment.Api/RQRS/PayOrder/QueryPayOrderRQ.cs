namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    /// <summary>
    /// 查询订单请求参数对象
    /// </summary>
    public class QueryPayOrderRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付系统订单号
        /// </summary>
        public string PayOrderId { get; set; }
    }
}
