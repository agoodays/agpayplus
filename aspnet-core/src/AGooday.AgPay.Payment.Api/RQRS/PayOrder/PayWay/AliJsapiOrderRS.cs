using AGooday.AgPay.Common.Constants;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_JSAPI
    /// </summary>
    public class AliJsapiOrderRS : UnifiedOrderRS
    {
        public string AlipayTradeNo { get; set; }

        public override string BuildPayDataType()
        {
            return CS.PAY_DATA_TYPE.ALI_APP;
        }

        public override string BuildPayData()
        {
            return JsonConvert.SerializeObject(new { alipayTradeNo = AlipayTradeNo });
        }
    }
}
