using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Manager.Api.WebSockets
{
    /// <summary>
    /// WebSocket服务类
    /// https://blog.darkthread.net/blog/aspnet-core-websocket-chatroom/
    /// https://www.cnblogs.com/kklldog/p/core-for-websocket.html
    /// </summary>
    public class WsChannelUserIdServer
    {
        // appId 与 WsChannelUserIdServer 存储关系, ConcurrentHashMap保证线程安全
        // REF: https://radu-matei.com/blog/aspnet-core-websockets-middleware/
        // 静态连接池（外层appId，内层cid）
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WsChannelUserIdServer>>
            _wsAppIdMap = new();

        private readonly ILogger<WsChannelUserIdServer> _logger;
        private WebSocket _webSocket;
        private string _cid;
        private string _appId;

        public WsChannelUserIdServer(ILogger<WsChannelUserIdServer> logger)
        {
            _logger = logger;
        }

        public async Task ProcessWebSocketAsync(WebSocket webSocket, string cid, string appId)
        {
            _webSocket = webSocket;
            _cid = cid;
            _appId = appId;

            try
            {
                RegisterConnection();
                await HandleWebSocketCommunicationAsync();
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                _logger.LogWarning("客户端异常断开: App={App}, CID={Cid}", _appId, _cid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket处理异常: App={App}, CID={Cid}", _appId, _cid);
            }
            finally
            {
                await SafeCleanupAsync();
            }
        }

        #region 连接池管理
        private void RegisterConnection()
        {
            var appConnections = _wsAppIdMap.GetOrAdd(_appId, _ => new ConcurrentDictionary<string, WsChannelUserIdServer>());

            if (!appConnections.TryAdd(_cid, this))
            {
                throw new InvalidOperationException($"App={_appId}, CID={_cid}, 已存在连接池中");
            }

            _logger.LogInformation("客户端连接已注册: App={App}, CID={Cid}, 当前连接数={Count}",
                _appId, _cid, appConnections.Count);
        }

        private void UnregisterConnection()
        {
            if (!_wsAppIdMap.TryGetValue(_appId, out var appConnections))
                return;

            if (!appConnections.TryRemove(_cid, out _))
                return;

            _logger.LogInformation("客户端连接已移除: App={App}, CID={Cid}, 剩余连接数={Count}",
                _appId, _cid, appConnections.Count);

            if (appConnections.IsEmpty)
            {
                _wsAppIdMap.TryRemove(_appId, out _);
                _logger.LogInformation("订单组已清空: App={App}", _appId);
            }
        }
        #endregion

        #region WebSocket通信处理
        private async Task HandleWebSocketCommunicationAsync()
        {
            var buffer = new byte[1024];
            WebSocketReceiveResult result;

            while (_webSocket.State == WebSocketState.Open)
            {
                result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await HandleCloseRequestAsync(result);
                    break;
                }

                // 处理其他消息类型（如文本消息）
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.LogDebug("收到消息: App={App}, CID={Cid}, Msg={Msg}",
                        _appId, _cid, message);
                }
            }
        }

        private async Task HandleCloseRequestAsync(WebSocketReceiveResult result)
        {
            try
            {
                await _webSocket.CloseAsync(
                    result.CloseStatus.Value,
                    result.CloseStatusDescription,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogDebug("关闭握手异常: {Message}", ex.Message);
            }
        }
        #endregion

        #region 资源清理
        private async Task SafeCleanupAsync()
        {
            try
            {
                if (_webSocket?.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "服务端关闭",
                        CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("关闭连接时发生次要异常: {Message}", ex.Message);
            }
            finally
            {
                UnregisterConnection();
                _webSocket?.Dispose();
                _webSocket = null;
            }
        }
        #endregion

        #region 消息发送
        public async Task SendMessageAsync(string message)
        {
            if (_webSocket?.State != WebSocketState.Open)
            {
                _logger.LogWarning("尝试发送消息到关闭的连接: App={App}, CID={Cid}", _appId, _cid);
                return;
            }

            try
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "消息发送失败: App={App}, CID={Cid}", _appId, _cid);
                await SafeCleanupAsync();
            }
        }

        public static async Task BroadcastToAppAndCidAsync(string appId, string cid, string message)
        {
            if (!_wsAppIdMap.TryGetValue(appId, out var appConnections))
                return;

            var tasks = appConnections.Values.Where(w => w._cid.Equals(cid)).Select(conn =>
            {
                try
                {
                    return conn.SendMessageAsync(message);
                }
                catch (Exception ex)
                {
                    conn._logger.LogError(ex, "广播消息异常: App={App}, CID={Cid}", conn._appId, conn._cid);
                    return Task.CompletedTask;
                }
            });
            LogUtil<WsChannelUserIdServer>.Info($"已广播消息到 {tasks.Count()} 个客户端: App={appId}");

            await Task.WhenAll(tasks);
        }
        #endregion
    }
}
