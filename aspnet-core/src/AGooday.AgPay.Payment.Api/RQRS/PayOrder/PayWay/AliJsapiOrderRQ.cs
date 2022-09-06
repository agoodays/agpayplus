using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
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
        private string buyerUserId { get; set; }

        /** 构造函数 **/
        public AliJsapiOrderRQ()
        {
            this.wayCode = "ALI_JSAPI";
        }

        public override string GetChannelUserId()
        {
            return this.buyerUserId;
        }
    }
}
