using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 统一下单
    /// </summary>
    [ApiController]
    public class UnifiedOrderController : AbstractPayOrderController
    {
        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/pay/unifiedOrder")]
        public ActionResult<ApiRes> UnifiedOrder()
        {
            return Ok();
        }
    }
}
