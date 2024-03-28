using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace AGooday.AgPay.Manager.Api.WebSockets
{
    /// <summary>
    /// WebSocket服务类
    /// https://blog.darkthread.net/blog/aspnet-core-websocket-chatroom/
    /// https://www.cnblogs.com/kklldog/p/core-for-websocket.html
    /// </summary>
    public class WsChannelUserIdServer
    {
        private readonly ILogger<WsChannelUserIdServer> _logger;

        public WsChannelUserIdServer(ILogger<WsChannelUserIdServer> logger)
        {
            _logger = logger;
        }

        //当前在线客户端 数量
        private static int OnlineClientSize = 0;

        // appId 与 WsChannelUserIdServer 存储关系, ConcurrentHashMap保证线程安全
        //REF: https://radu-matei.com/blog/aspnet-core-websockets-middleware/
        private readonly ConcurrentDictionary<string, ISet<WsChannelUserIdServer>> wsAppIdMap = new ConcurrentDictionary<string, ISet<WsChannelUserIdServer>>();

        //与某个客户端的连接会话，需要通过它来给客户端发送数据
        private WebSocket ClientSession;

        //客户端自定义ID
        private string Cid = "";

        //应用ID
        private string AppId = "";

        public async Task ProcessWebSocket(WebSocket webSocket, string cid, string appId)
        {
            try
            {
                //设置当前属性
                this.Cid = cid;
                this.AppId = appId;
                this.ClientSession = webSocket;

                var isExist = wsAppIdMap.TryGetValue(this.AppId, out ISet<WsChannelUserIdServer> wsServerSet);
                if (!isExist)
                {
                    wsServerSet = new HashSet<WsChannelUserIdServer>();
                }
                wsServerSet.Add(this);
                wsAppIdMap.TryAdd(this.AppId, wsServerSet);

                AddOnlineCount(); //在线数加1

                WebSocketReceiveResult result = null;
                do
                {
                    var buffer = new byte[1024 * 1];
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                    {
                        var msgString = Encoding.UTF8.GetString(buffer);
                        _logger.LogInformation($"Websocket客户端cid[{this.Cid}],appId[{this.AppId}]接收异步消息：{msgString}.");
                    }
                    else
                    {
                        wsServerSet.Remove(this);
                        if (!wsServerSet.Any())
                        {
                            wsAppIdMap.TryRemove(this.AppId, out ISet<WsChannelUserIdServer> wsSet);
                        }
                        this.SubOnlineCount();
                        _logger.LogInformation($"Websocket客户端cid[{this.Cid}],appId[{this.AppId}],{this.GetOnlineClientSize()}");
                    }
                }
                while (!result.CloseStatus.HasValue);

                _logger.LogInformation($"cid[{cid}],appId[{appId}]连接开启监听！当前在线人数为{OnlineClientSize}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"ws监听异常cid[{cid}],appId[{appId}]");
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
        /// <param name="appId"></param>
        /// <param name="msg"></param>
        public async Task SendMsgByAppAndCid(string appId, string cid, string msg)
        {
            try
            {
                _logger.LogInformation($"推送ws消息到浏览器, appId={cid}, appId={cid}，msg={msg}");

                var isExist = wsAppIdMap.TryGetValue(appId, out ISet<WsChannelUserIdServer> wsSet);
                if (!isExist)
                {
                    _logger.LogInformation($"appId[{appId}] 无ws监听客户端");
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
                        _logger.LogInformation(e, $"推送设备消息时异常，appId={appId}, cid={item.Cid}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, $"推送消息时异常，appId={appId}");
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
