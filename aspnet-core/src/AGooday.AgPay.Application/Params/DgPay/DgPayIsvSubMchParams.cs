namespace AGooday.AgPay.Application.Params.DgPay
{
    /// <summary>
    /// 斗拱 特约商户参数定义
    /// </summary>
    public class DgPayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 汇付客户Id
        /// </summary>
        public string HuifuId { get; set; }

        public string SubMchLiteAppId { get; set; }
    }
}
