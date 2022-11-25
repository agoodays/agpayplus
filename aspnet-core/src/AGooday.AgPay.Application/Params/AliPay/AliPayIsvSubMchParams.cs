namespace AGooday.AgPay.Application.Params.AliPay
{
    /// <summary>
    /// 支付宝 特约商户参数定义
    /// </summary>
    public class AliPayIsvSubMchParams : IsvSubMchParams
    {
        public string AppAuthToken { get; set; }
    }
}
