using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_APP
    /// </summary>
    public class AliAppOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AliAppOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.ALI_APP; //默认 wayCode, 避免validate出现问题
        }
    }
}
