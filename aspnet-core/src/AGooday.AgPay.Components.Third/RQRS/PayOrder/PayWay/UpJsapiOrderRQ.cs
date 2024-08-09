using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： UP_JSAPI
    /// </summary>
    public class UpJsapiOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UpJsapiOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.UP_JSAPI;
        }
    }
}
