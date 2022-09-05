using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 创建支付订单抽象类
    /// </summary>
    public abstract class AbstractPayOrderController : ControllerBase
    {
        /// <summary>
        /// 统一下单 (新建订单模式)
        /// </summary>
        /// <param name="wayCode"></param>
        /// <param name="bizRQ">业务请求报文</param>
        /// <returns></returns>
        protected ActionResult UnifiedOrder(string wayCode, object? bizRQ)
        {
            return Ok();
        }
    }
}
