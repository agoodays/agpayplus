using AGooday.AgPay.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_BAR
    /// </summary>
    public class AliBarOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 用户 支付条码
        /// </summary>
        [Required(ErrorMessage = "支付条码不能为空")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AliBarOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.ALI_BAR; //默认 ali_bar, 避免validate出现问题
        }
    }
}
