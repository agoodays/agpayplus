namespace AGooday.AgPay.Application.Params.YsePay
{
    /// <summary>
    /// 银盛 通用配置信息
    /// </summary>
    public class YsePayConfig
    {
        /// <summary>
        /// 网关地址
        /// </summary>
        public const string QRCODE_GATEWAY = "https://openapi.ysepay.com/gateway.do";
        public const string OPENAPI_GATEWAY = "https://qrcode.ysepay.com/gateway.do";
        public const string SEARCH_GATEWAY = "https://search.ysepay.com/gateway.do";
    }
}
