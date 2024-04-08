namespace AGooday.AgPay.Application.Params.LklPay
{
    /// <summary>
    /// 拉卡拉 特约商户参数定义
    /// </summary>
    public class LklPayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantNo { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string TermNo { get; set; }

        /// <summary>
        /// 微信子商户号（subMchId）
        /// </summary>
        public string SubMchId { get; set; }
    }
}
