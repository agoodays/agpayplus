using AGooday.AgPay.Manager.Api.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.WebSockets
{
    [ApiController]
    public class WsChannelUserIdController : ControllerBase
    {
        private readonly WsChannelUserIdServer _wsChannelUserIdServer;

        public WsChannelUserIdController(WsChannelUserIdServer webSocketHandler)
        {
            _wsChannelUserIdServer = webSocketHandler;
        }

        /// <summary>
        /// /ws/channelUserId/{appId}/{客戶端自定義ID}
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="cid">客戶端自定義ID</param>
        /// <returns></returns>
        [HttpGet, Route("api/anon/ws/channelUserId/{appId}/{cid}")]
        public async Task Get(string appId, string cid)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _wsChannelUserIdServer.ProcessWebSocket(webSocket, cid, appId);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
