using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： UPACP_WAP
    /// </summary>
    public class UpWapOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UpWapOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.UP_WAP; //默认 wayCode, 避免validate出现问题
        }
    }
}
