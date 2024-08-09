using AGooday.AgPay.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_JSAPI
    /// </summary>
    public class AliJsapiOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 支付宝用户ID
        /// </summary>
        /// <param name=""></param>
        [Required(ErrorMessage = "用户ID不能为空")]
        public string BuyerUserId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AliJsapiOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.ALI_JSAPI;
        }

        public override string GetChannelUserId()
        {
            return this.BuyerUserId;
        }
    }
}
