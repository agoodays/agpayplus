namespace AGooday.AgPay.Application.Params.AllinPay
{
    public class AllinPayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Cusid { get; set; }

        /// <summary>
        /// 商户终端号（商户下唯一）
        /// </summary>
        public string MchTrmNo { get; set; }
    }
}
