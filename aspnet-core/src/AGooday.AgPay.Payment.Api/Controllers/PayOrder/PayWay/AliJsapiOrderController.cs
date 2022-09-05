using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder.PayWay
{
    /// <summary>
    /// 支付宝 jspai
    /// </summary>
    [ApiController]
    public class AliJsapiOrderController : AbstractPayOrderController
    {
        /// <summary>
        /// 统一下单接口
        /// </summary>
        [HttpPost]
        [Route("api/pay/aliJsapiOrder")]
        public ActionResult AliJsapiOrder()
        {
            // 统一下单接口
            // "ALI_JSAPI";  //支付宝服务窗支付
            return UnifiedOrder("ALI_JSAPI", null);
        }
    }
}
