using System.ComponentModel.DataAnnotations;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： WX_BAR
    /// </summary>
    public class WxBarOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 用户 支付条码
        /// </summary>
        [Required(ErrorMessage = "支付条码不能为空")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WxBarOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.WX_BAR; //默认 wx_bar, 避免validate出现问题
        }
    }
}
