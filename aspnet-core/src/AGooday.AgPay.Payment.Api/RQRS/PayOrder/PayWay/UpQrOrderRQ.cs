using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： UPACP_QR
    /// </summary>
    public class UpQrOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UpQrOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.UP_QR; //默认 wayCode, 避免validate出现问题
        }
    }
}
