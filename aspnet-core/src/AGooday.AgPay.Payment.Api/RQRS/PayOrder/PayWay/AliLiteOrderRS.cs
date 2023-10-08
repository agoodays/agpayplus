using AGooday.AgPay.Common.Constants;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_LITE
    /// </summary>
    public class AliLiteOrderRS : UnifiedOrderRS
    {
        /// <summary>
        /// 调起支付插件的支付宝订单号
        /// </summary>
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
