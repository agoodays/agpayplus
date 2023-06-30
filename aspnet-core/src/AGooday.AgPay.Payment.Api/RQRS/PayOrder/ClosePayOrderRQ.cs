namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    /// <summary>
    /// 关闭订单 请求参数对象
    /// </summary>
    public class ClosePayOrderRQ : AbstractMchAppRQ
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
