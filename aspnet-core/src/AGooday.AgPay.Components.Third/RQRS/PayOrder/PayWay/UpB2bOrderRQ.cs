using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： UPACP_B2B
    /// </summary>
    public class UpB2bOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UpB2bOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.UP_B2B; //默认 wayCode, 避免validate出现问题
        }
    }
}
