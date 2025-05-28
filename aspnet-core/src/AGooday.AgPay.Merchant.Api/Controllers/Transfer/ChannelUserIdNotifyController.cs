using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Transfer
{
    [Route("api/anon/channelUserIdCallback")]
    [ApiController, AllowAnonymous, NoLog]
    public class ChannelUserIdNotifyController : Controller
    {
        private readonly ILogger<ChannelUserIdNotifyController> _logger;
        private readonly WsChannelUserIdServer _wsChannelUserIdServer;

        public ChannelUserIdNotifyController(ILogger<ChannelUserIdNotifyController> logger,
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
                await WsChannelUserIdServer.BroadcastToAppAndCidAsync(appId, extParam, channelUserId);
            }
            catch (Exception e)
            {
                ViewBag.ErrMsg = e.Message;
            }
            return View("~/Views/ChannelUser/GetChannelUserIdPage.cshtml");
        }
    }
}
