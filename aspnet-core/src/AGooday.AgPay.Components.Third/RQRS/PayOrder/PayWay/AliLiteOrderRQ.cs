using AGooday.AgPay.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_LITE
    /// </summary>
    public class AliLiteOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 支付宝用户ID
        /// </summary>
        [Required(ErrorMessage = "用户ID不能为空")]
        public string BuyerUserId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AliLiteOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.ALI_LITE;
        }

        public override string GetChannelUserId()
        {
            return this.BuyerUserId;
        }
    }
}
