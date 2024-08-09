using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： UPACP_APP
    /// </summary>
    public class UpAppOrderRS : CommonPayDataRS
    {
        //private string PayData { get; set; }

        public override string BuildPayDataType()
        {
            return CS.PAY_DATA_TYPE.YSF_APP;
        }

        public override string BuildPayData()
        {
            return PayData;
        }
    }
}
