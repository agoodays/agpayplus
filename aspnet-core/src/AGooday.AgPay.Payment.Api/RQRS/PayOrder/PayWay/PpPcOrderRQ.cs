using AGooday.AgPay.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： PP_PC
    /// </summary>
    public class PpPcOrderRQ : CommonPayDataRQ
    {
        [Required(ErrorMessage = "取消支付返回站点")]
        public string CancelUrl { get; set; }

        /** 构造函数 **/
        public PpPcOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.PP_PC;
        }
    }
}
