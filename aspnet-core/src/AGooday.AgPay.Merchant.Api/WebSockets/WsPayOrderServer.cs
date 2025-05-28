namespace AGooday.AgPay.Merchant.Api.WebSockets
{
    /// <summary>
    /// 支付订单WebSocket服务
    /// </summary>
    public class WsPayOrderServer : AbstractWebSocketServer<string>
    {
        public WsPayOrderServer(ILogger<WsPayOrderServer> logger) : base(logger)
        {
        }
    }
}