using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_PC
    /// </summary>
    public class AliPcOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AliPcOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.ALI_PC;
        }
    }
}
