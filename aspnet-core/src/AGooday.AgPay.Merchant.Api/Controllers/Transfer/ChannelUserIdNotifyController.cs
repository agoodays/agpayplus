using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Controllers.PayTest;
using AGooday.AgPay.Merchant.Api.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Transfer
{
    [Route("api/anon/channelUserIdCallback")]
    [ApiController, AllowAnonymous, NoLog]
    public class ChannelUserIdNotifyController : Controller
    {
        private readonly ILogger<PayTestNotifyController> _logger;
        private readonly WsChannelUserIdServer _wsChannelUserIdServer;

        public ChannelUserIdNotifyController(ILogger<PayTestNotifyController> logger,
            WsChannelUserIdServer wsChannelUserIdServer)
        {
            _logger = logger;
            _wsChannelUserIdServer = wsChannelUserIdServer;
        }

        [HttpGet, Route("")]
        public async Task<ActionResult> ChannelUserId(string appId, string channelUserId, string extParam)
        {
            try
            {
                // WebSocket 推送到前端
                await _wsChannelUserIdServer.SendMsgByAppAndCid(appId, extParam, channelUserId);
            }
            catch (Exception e)
            {
                ViewBag.ErrMsg = e.Message;
            }
            return View("~/Views/ChannelUser/GetChannelUserIdPage.cshtml");
        }
    }
}
