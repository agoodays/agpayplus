namespace AGooday.AgPay.Payment.Api.RQRS.Transfer
{
    /// <summary>
    /// 查询转账单请求参数对象
    /// </summary>
    public class QueryTransferOrderRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 商户转账单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付系统转账单号
        /// </summary>
        public string TransferId { get; set; }
    }
}
