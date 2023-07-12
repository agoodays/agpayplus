namespace AGooday.AgPay.Application.Params.UmsPay
{
    public class UmsPayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Mid { get; set; }
        
        /// <summary>
        /// 终端号
        /// </summary>
        public string Tid { get; set; }
    }
}
