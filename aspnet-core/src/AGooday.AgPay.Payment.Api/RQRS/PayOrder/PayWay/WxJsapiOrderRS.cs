using AGooday.AgPay.Common.Constants;
using System.Drawing.Drawing2D;
using System.Security.Cryptography.Xml;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： WX_JSAPI
    /// </summary>
    public class WxJsapiOrderRS : UnifiedOrderRS
    {
        /// <summary>
        /// 预支付数据包
        /// </summary>
        public string PayInfo { get; set; }

        public override string BuildPayDataType()
        {
            return CS.PAY_DATA_TYPE.WX_APP;
        }

        public override string BuildPayData()
        {
            return PayInfo;
        }
    }
}
