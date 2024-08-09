using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_OC
    /// </summary>
    public class AliOcOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AliOcOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.ALI_OC;
        }
    }
}
