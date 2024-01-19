using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace AGooday.AgPay.Merchant.Api.WebSockets
{
    /// <summary>
    /// WebSocket服务类
    /// https://blog.darkthread.net/blog/aspnet-core-websocket-chatroom/
    /// https://www.cnblogs.com/kklldog/p/core-for-websocket.html
    /// </summary>
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
        private readonly ConcurrentDictionary<string, ISet<WsPayOrderServer>> wsOrderIdMap = new ConcurrentDictionary<string, ISet<WsPayOrderServer>>();

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

                var isExist = wsOrderIdMap.TryGetValue(this.PayOrderId, out ISet<WsPayOrderServer> wsServerSet);
                if (!isExist)
                {
                    wsServerSet = new HashSet<WsPayOrderServer>();
                }
                wsServerSet.Add(this);
                wsOrderIdMap.TryAdd(this.PayOrderId, wsServerSet);

                AddOnlineCount(); //在线数加1

                WebSocketReceiveResult result = null;
                do
                {
                    var buffer = new byte[1024 * 1];
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                    {
                        var msgString = Encoding.UTF8.GetString(buffer);
                        logger.LogInformation($"Websocket客户端cid[{this.Cid}],payOrderId[{this.PayOrderId}]接收异步消息：{msgString}.");
                    }
                    else
                    {
                        wsServerSet.Remove(this);
                        if (!wsServerSet.Any())
                        {
                            wsOrderIdMap.TryRemove(this.PayOrderId, out ISet<WsPayOrderServer> wsSet);
                        }
                        this.SubOnlineCount();
                        logger.LogInformation($"Websocket客户端cid[{this.Cid}],payOrderId[{this.PayOrderId}],{this.GetOnlineClientSize()}");
                    }
                }
                while (!result.CloseStatus.HasValue);

                logger.LogInformation($"cid[{cid}],payOrderId[{payOrderId}]连接开启监听！当前在线人数为{OnlineClientSize}");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"ws监听异常cid[{cid}],payOrderId[{payOrderId}]");
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

                var isExist = wsOrderIdMap.TryGetValue(payOrderId, out ISet<WsPayOrderServer> wsSet);
                if (!isExist)
                {
                    logger.LogInformation($"payOrderId[{payOrderId}] 无ws监听客户端");
                    return;
                }

                foreach (var item in wsSet)
                {
                    try
                    {
                        await item.SendMessage(msg);
                    }
                    catch (Exception e)
                    {
                        logger.LogInformation(e, $"推送设备消息时异常，payOrderId={payOrderId}, cid={item.Cid}");
                    }
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
