using AGooday.AgPay.Merchant.Api.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Transfer
{
    [Route("api/anon/channelUserIdCallback")]
    [ApiController, AllowAnonymous, NoLog]
    public class ChannelUserIdNotifyController : Controller
    {
        [HttpGet, Route("")]
        public ActionResult ChannelUserId(string appId, string channelUserId, string extParam)
        {
            try
            {
                // WebSocket 推送到前端

            }
            catch (Exception e)
            {
                ViewBag.ErrMsg = e.Message;
            }
            return View("~/Views/ChannelUser/GetChannelUserIdPage.cshtml");
        }
    }
}
