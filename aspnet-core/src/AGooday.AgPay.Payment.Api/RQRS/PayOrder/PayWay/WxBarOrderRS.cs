using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    public class WxBarOrderRS : UnifiedOrderRS
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
