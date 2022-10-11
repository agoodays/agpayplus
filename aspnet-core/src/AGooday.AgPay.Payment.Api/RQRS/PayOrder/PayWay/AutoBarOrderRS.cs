using AGooday.AgPay.Common.Constants;
using System.Drawing.Drawing2D;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： AUTO_BAR
    /// </summary>
    public class AutoBarOrderRS : UnifiedOrderRS
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
