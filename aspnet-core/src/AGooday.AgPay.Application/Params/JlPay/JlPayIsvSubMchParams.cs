namespace AGooday.AgPay.Application.Params.JlPay
{
    /// <summary>
    /// 嘉联 特约商户参数定义
    /// </summary>
    public class JlPayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string TermNo { get; set; }

        public string SubMchLiteAppId { get; set; }
    }
}
