using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder.PayWay
{
    /// <summary>
    /// 支付宝 条码支付
    /// </summary>
    [ApiController]
    public class AliBarOrderController : AbstractPayOrderController
    {
        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/pay/aliBarOrder")]
        public ActionResult<ApiRes> AliBarOrder(AliBarOrderRQ bizRQ)
        {
            // 统一下单接口;
            return UnifiedOrder(CS.PAY_WAY_CODE.ALI_BAR, bizRQ);// "ALI_BAR";  //支付宝条码支付
        }
    }
}
