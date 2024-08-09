using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： WX_NATIVE
    /// </summary>
    public class WxNativeOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WxNativeOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.WX_NATIVE;
        }
    }
}
