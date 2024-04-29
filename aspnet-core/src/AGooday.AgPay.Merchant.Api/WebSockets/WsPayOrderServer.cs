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
        private readonly ILogger<WsPayOrderServer> _logger;

        public WsPayOrderServer(ILogger<WsPayOrderServer> logger)
        {
            _logger = logger;
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
                        var msg = Encoding.UTF8.GetString(buffer);
                        _logger.LogInformation($"Websocket客户端Cid[{this.Cid}], PayOrderId[{this.PayOrderId}]接收异步消息: {msg}.");
                    }
                    else
                    {
                        wsServerSet.Remove(this);
                        if (!wsServerSet.Any())
                        {
                            wsOrderIdMap.TryRemove(this.PayOrderId, out ISet<WsPayOrderServer> wsSet);
                        }
                        this.SubOnlineCount();
                        _logger.LogInformation($"Websocket客户端Cid[{this.Cid}], PayOrderId[{this.PayOrderId}], 当前在线人数为: {this.GetOnlineClientSize()}");
                    }
                }
                while (!result.CloseStatus.HasValue);

                _logger.LogInformation($"Websocket客户端Cid[{this.Cid}], PayOrderId[{this.PayOrderId}]连接开启监听！当前在线人数为: {OnlineClientSize}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Websocket监听异常, 客户端Cid[{this.Cid}], PayOrderId[{this.PayOrderId}]");
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
                _logger.LogInformation($"推送ws消息到浏览器, PayOrderId={payOrderId}, Msg={msg}");

                var isExist = wsOrderIdMap.TryGetValue(payOrderId, out ISet<WsPayOrderServer> wsSet);
                if (!isExist)
                {
                    _logger.LogInformation($"PayOrderId[{payOrderId}] 无ws监听客户端");
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
                        _logger.LogInformation(e, $"推送设备消息时异常, PayOrderId={payOrderId}, Cid={item.Cid}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, $"推送消息时异常, PayOrderId={payOrderId}");
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
