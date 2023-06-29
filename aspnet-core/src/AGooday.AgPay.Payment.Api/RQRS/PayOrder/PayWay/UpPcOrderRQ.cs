using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： UPACP_PC
    /// </summary>
    public class UpPcOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UpPcOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.UP_PC; //默认 wayCode, 避免validate出现问题
        }
    }
}
