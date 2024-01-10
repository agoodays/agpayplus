namespace AGooday.AgPay.Application.Params.YsfPay
{
    /// <summary>
    /// 云闪付 配置信息
    /// </summary>
    public class YsfPayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 商户编号
        /// </summary>
        public string MerId { get; set; }

        public string SubMchAppId { get; set; }
    }
}
