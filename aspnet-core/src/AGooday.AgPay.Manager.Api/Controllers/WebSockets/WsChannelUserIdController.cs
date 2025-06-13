using AGooday.AgPay.Manager.Api.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.WebSockets
{
    [Route("api/anon")]
    [ApiController, AllowAnonymous]
    public class WsChannelUserIdController : ControllerBase
    {
        private readonly WsChannelUserIdServer _wsChannelUserIdServer;

        public WsChannelUserIdController(WsChannelUserIdServer webSocketHandler)
        {
            _wsChannelUserIdServer = webSocketHandler;
        }

        /// <summary>
        /// ws/channelUserId/{appId}/{客户端自定义ID}
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="cid">客户端自定义ID</param>
        /// <returns></returns>
        [HttpGet, Route("ws/channelUserId/{appId}/{cid}")]
        public async Task GetAsync(string appId, string cid)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _wsChannelUserIdServer.ProcessWebSocketAsync(webSocket, cid, (appId, cid));
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
