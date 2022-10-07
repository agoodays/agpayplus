using AGooday.AgPay.Common.Constants;
using System.Drawing.Drawing2D;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    public class AliAppOrderRS : UnifiedOrderRS
    {
        //private string PayData { get; set; }

        public override string BuildPayDataType()
        {
            return CS.PAY_DATA_TYPE.ALI_APP;
        }

        public override string BuildPayData()
        {
            return PayData;
        }
    }
}
