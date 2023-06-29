using AGooday.AgPay.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： WX_APP
    /// </summary>
    public class WxAppOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 微信openid
        /// </summary>
        [Required(ErrorMessage = "openid不能为空")]
        public string Openid { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WxAppOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.WX_APP; //默认 wayCode, 避免validate出现问题
        }

        public override string GetChannelUserId()
        {
            return this.Openid;
        }
    }
}
