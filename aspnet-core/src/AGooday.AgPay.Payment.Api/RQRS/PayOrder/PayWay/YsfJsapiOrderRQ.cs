using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： YSF_JSAPI
    /// </summary>
    public class YsfJsapiOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public YsfJsapiOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.YSF_JSAPI;
        }
    }
}
