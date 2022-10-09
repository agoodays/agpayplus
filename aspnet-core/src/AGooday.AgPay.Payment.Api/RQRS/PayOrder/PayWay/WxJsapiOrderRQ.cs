using AGooday.AgPay.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： WX_JSAPI
    /// </summary>
    public class WxJsapiOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 用户 支付条码
        /// </summary>
        [Required(ErrorMessage = "openid不能为空")]
        public string Openid { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WxJsapiOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.WX_JSAPI;
        }
        public override string GetChannelUserId()
        {
            return this.Openid;
        }
    }
}
