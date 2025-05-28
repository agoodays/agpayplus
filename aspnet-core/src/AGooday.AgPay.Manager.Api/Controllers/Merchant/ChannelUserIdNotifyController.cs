using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
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
                await _wsChannelUserIdServer.BroadcastToGroupAsync((appId, extParam), channelUserId);
            }
            catch (Exception e)
            {
                ViewBag.ErrMsg = e.Message;
            }
            return View("~/Views/ChannelUser/GetChannelUserIdPage.cshtml");
        }
    }
}
