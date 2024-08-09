using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： AUTO_BAR
    /// </summary>
    public class AutoBarOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 条码值
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AutoBarOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.AUTO_BAR;
        }
    }
}
