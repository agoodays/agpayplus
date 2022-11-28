using AGooday.AgPay.Merchant.Api.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.WebSockets
{
    // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/websockets?view=aspnetcore-6.0
    [ApiController]
    public class WsPayOrderController : ControllerBase
    {
        private readonly WsPayOrderServer _wsPayOrderServer;

        public WsPayOrderController(WsPayOrderServer webSocketHandler)
        {
            _wsPayOrderServer = webSocketHandler;
        }

        /// <summary>
        /// /ws/payOrder/{訂單ID}/{客戶端自定義ID}
        /// </summary>
        /// <param name="payOrderId">訂單ID</param>
        /// <param name="cid">客戶端自定義ID</param>
        /// <returns></returns>
        [HttpGet, Route("api/anon/ws/payOrder/{payOrderId}/{cid}")]
        public async Task Get(string payOrderId, string cid)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _wsPayOrderServer.ProcessWebSocket(webSocket, cid, payOrderId);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
