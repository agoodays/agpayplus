namespace AGooday.AgPay.Application.Params.YsePay
{
    /// <summary>
    /// 银盛 特约商户参数定义
    /// </summary>
    public class YsePayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 收款商户号
        /// </summary>
        public string SellerId { get; set; }

        /// <summary>
        /// 收款商户号对应商户名称
        /// </summary>
        public string SellerName { get; set; }
    }
}
