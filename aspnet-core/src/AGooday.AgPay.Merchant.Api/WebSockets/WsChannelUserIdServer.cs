namespace AGooday.AgPay.Merchant.Api.WebSockets
{
    /// <summary>
    /// 渠道用户WebSocket服务
    /// </summary>
    public class WsChannelUserIdServer : AbstractWebSocketServer<(string AppId, string Cid)>
    {
        public WsChannelUserIdServer(ILogger<WsChannelUserIdServer> logger) : base(logger)
        {
        }
    }
}