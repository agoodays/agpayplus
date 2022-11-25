using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： YSF_BAR
    /// </summary>
    public class YsfBarOrderRS : UnifiedOrderRS
    {
        public override string BuildPayDataType()
        {
            return CS.PAY_DATA_TYPE.NONE;
        }

        public override string BuildPayData()
        {
            return "";
        }
    }
}
