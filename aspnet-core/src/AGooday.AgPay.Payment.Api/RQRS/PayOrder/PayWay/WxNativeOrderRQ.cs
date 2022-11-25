using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： WX_NATIVE
    /// </summary>
    public class WxNativeOrderRQ : CommonPayDataRQ
    {
        /** 构造函数 **/
        public WxNativeOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.WX_NATIVE;
        }
    }
}
