using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_WAP
    /// </summary>
    public class AliWapOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AliWapOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.ALI_WAP;//默认 ALI_WAP, 避免validate出现问题
        }
    }
}
