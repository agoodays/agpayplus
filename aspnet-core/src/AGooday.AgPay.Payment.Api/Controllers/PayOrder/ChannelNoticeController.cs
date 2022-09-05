using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 渠道侧的通知入口Controller 【分为同步跳转（doReturn）和异步回调(doNotify) 】
    /// </summary>
    [ApiController]
    [Route("api/pay")]
    public class ChannelNoticeController : ControllerBase
    {
        /// <summary>
        /// 同步通知入口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/pay/return/{ifCode}")]
        [Route("/api/pay/return/{ifCode}/{payOrderId}")]
        public ActionResult DoReturn()
        {
            return Ok();
        }

        /// <summary>
        /// 异步回调入口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/pay/notify/{ifCode}")]
        [Route("/api/pay/notify/{ifCode}/{payOrderId}")]
        public ActionResult DoNotify()
        {
            return Ok();
        }
    }
}
