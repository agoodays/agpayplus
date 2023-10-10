using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： UP_JSAPI
    /// </summary>
    public class UpJsapiOrderRS : UnifiedOrderRS
    {
        /// <summary>
        /// 调起支付插件的云闪付订单号
        /// </summary>
        public string RedirectUrl { get; set; }

        public override string BuildPayDataType()
        {
            return CS.PAY_DATA_TYPE.YSF_APP;
        }

        public override string BuildPayData()
        {
            return JsonUtil.NewJson("redirectUrl", RedirectUrl).ToString();
        }
    }
}
