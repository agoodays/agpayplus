using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace AGooday.AgPay.Merchant.Api.WebSockets
{
    // https://blog.darkthread.net/blog/aspnet-core-websocket-chatroom/
    // https://www.cnblogs.com/kklldog/p/core-for-websocket.html
    public class WsPayOrderServer
    {
        private readonly ILogger<WsPayOrderServer> logger;

        public WsPayOrderServer(ILogger<WsPayOrderServer> logger)
        {
            this.logger = logger;
        }

        //当前在线客户端 数量
        private static int OnlineClientSize = 0;

        // payOrderId 与 WsPayOrderServer 存储关系, ConcurrentHashMap保证线程安全
        //REF: https://radu-matei.com/blog/aspnet-core-websockets-middleware/
        ConcurrentDictionary<int, WebSocket> WebSockets = new ConcurrentDictionary<int, WebSocket>();
        ConcurrentDictionary<string, WsPayOrderServer> wsOrderIdMap = new ConcurrentDictionary<string, WsPayOrderServer>();

        //与某个客户端的连接会话，需要通过它来给客户端发送数据
        private WebSocket ClientSession;

        //客户端自定义ID
        private string Cid = "";

        //支付订单号
        private string PayOrderId = "";

        public async Task ProcessWebSocket(WebSocket webSocket, string cid, string payOrderId)
        {
            try
            {
                //设置当前属性
                this.Cid = cid;
                this.PayOrderId = payOrderId;
                this.ClientSession = webSocket;

                await Handle();

                logger.LogInformation($"cid[{cid}],payOrderId[{payOrderId}]连接开启监听！当前在线人数为{OnlineClientSize}");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"ws监听异常cid[{cid}],payOrderId[{payOrderId}]");
            }
        }

        private async Task Handle()
        {
            var isExist = wsOrderIdMap.TryAdd(this.PayOrderId,this);
            if (!isExist)
            {
                AddOnlineCount(); //在线数加1

                WebSocketReceiveResult result = null;
                do
                {
                    var buffer = new byte[1024 * 1];
                    result = await this.ClientSession.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                    {
                        var msgString = Encoding.UTF8.GetString(buffer);
                        logger.LogInformation($"Websocket客户端接收异步消息 {msgString}.");
                    }
                    else
                    {
                        var isSuccess = wsOrderIdMap.TryRemove(this.PayOrderId, out WsPayOrderServer _);
                        if (!isSuccess)
                        {
                            SubOnlineCount();
                        }
                    }
                }
                while (!result.CloseStatus.HasValue);
            }
        }

        /// <summary>
        /// 实现服务器主动推送
        /// </summary>
        /// <param name="message"></param>
        public async Task SendMessage(string message)
        {
            var buff = Encoding.UTF8.GetBytes(message);
            var data = new ArraySegment<byte>(buff, 0, buff.Length);
            var webSocket = this.ClientSession;
            if (webSocket.State == WebSocketState.Open)
                await webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// 根据订单ID,推送消息
        /// 捕捉所有的异常，避免影响业务。
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="msg"></param>
        public async Task SendMsgByOrderId(string payOrderId, string msg)
        {
            try
            {
                logger.LogInformation($"推送ws消息到浏览器, payOrderId={payOrderId}，msg={msg}");

                wsOrderIdMap.TryGetValue(payOrderId, out WsPayOrderServer wsSet);
                if (wsSet == null)
                {
                    logger.LogInformation($"payOrderId[{payOrderId}] 无ws监听客户端");
                    return;
                }

                try
                {
                    await wsSet.SendMessage(msg);
                }
                catch (Exception e)
                {
                    logger.LogInformation(e, $"推送设备消息时异常，payOrderId={payOrderId}, cid={wsSet.Cid}");
                }
            }
            catch (Exception e)
            {
                logger.LogInformation(e, $"推送消息时异常，payOrderId={payOrderId}");
            }
        }

        public int GetOnlineClientSize()
        {
            return OnlineClientSize;
        }

        public void AddOnlineCount()
        {
            OnlineClientSize++;
        }

        public void SubOnlineCount()
        {
            OnlineClientSize--;
        }
    }
}
