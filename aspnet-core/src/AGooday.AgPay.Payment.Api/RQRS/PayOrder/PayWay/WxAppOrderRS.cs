using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： WX_APP
    /// </summary>
    public class WxAppOrderRS : UnifiedOrderRS
    {
        /// <summary>
        /// 预支付数据包
        /// </summary>
        private string payInfo;

        public override string BuildPayDataType()
        {
            return CS.PAY_DATA_TYPE.WX_APP;
        }

        public override string BuildPayData()
        {
            return payInfo;
        }
    }
}
