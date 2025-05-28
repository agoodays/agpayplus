using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Merchant.Api.WebSockets
{
    /// <summary>
    /// WebSocket服务类
    /// https://blog.darkthread.net/blog/aspnet-core-websocket-chatroom/
    /// https://www.cnblogs.com/kklldog/p/core-for-websocket.html
    /// </summary>
    public class WsPayOrderServer
    {
        // payOrderId 与 WsPayOrderServer 存储关系, ConcurrentHashMap保证线程安全
        // REF: https://radu-matei.com/blog/aspnet-core-websockets-middleware/
        // 静态连接池（外层payOrderId，内层cid）
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WsPayOrderServer>>
            _wsOrderIdMap = new();

        private readonly ILogger<WsPayOrderServer> _logger;
        private WebSocket _webSocket;
        private string _cid;
        private string _payOrderId;

        public WsPayOrderServer(ILogger<WsPayOrderServer> logger)
        {
            _logger = logger;
        }

        public async Task ProcessWebSocketAsync(WebSocket webSocket, string cid, string payOrderId)
        {
            _webSocket = webSocket;
            _cid = cid;
            _payOrderId = payOrderId;

            try
            {
                RegisterConnection();
                await HandleWebSocketCommunicationAsync();
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                _logger.LogWarning("客户端异常断开: Order={Order}, CID={Cid}", _payOrderId, _cid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket处理异常: Order={Order}, CID={Cid}", _payOrderId, _cid);
            }
            finally
            {
                await SafeCleanupAsync();
            }
        }

        #region 连接池管理
        private void RegisterConnection()
        {
            var orderConnections = _wsOrderIdMap.GetOrAdd(_payOrderId, _ => new ConcurrentDictionary<string, WsPayOrderServer>());

            if (!orderConnections.TryAdd(_cid, this))
            {
                throw new InvalidOperationException($"Order={_payOrderId}, CID={_cid}, 已存在连接池中");
            }

            _logger.LogInformation("客户端连接已注册: Order={Order}, CID={Cid}, 当前连接数={Count}",
                _payOrderId, _cid, orderConnections.Count);
        }

        private void UnregisterConnection()
        {
            if (!_wsOrderIdMap.TryGetValue(_payOrderId, out var orderConnections))
                return;

            if (!orderConnections.TryRemove(_cid, out _))
                return;

            _logger.LogInformation("客户端连接已移除: Order={Order}, CID={Cid}, 剩余连接数={Count}",
                _payOrderId, _cid, orderConnections.Count);

            if (orderConnections.IsEmpty)
            {
                _wsOrderIdMap.TryRemove(_payOrderId, out _);
                _logger.LogInformation("订单组已清空: Order={Order}", _payOrderId);
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
                    _logger.LogDebug("收到消息: Order={Order}, CID={Cid}, Msg={Msg}",
                        _payOrderId, _cid, message);
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
                _logger.LogWarning("尝试发送消息到关闭的连接: Order={Order}, CID={Cid}", _payOrderId, _cid);
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
                _logger.LogError(ex, "消息发送失败: Order={Order}, CID={Cid}", _payOrderId, _cid);
                await SafeCleanupAsync();
            }
        }

        public static async Task BroadcastToOrderAsync(string payOrderId, string message)
        {
            if (!_wsOrderIdMap.TryGetValue(payOrderId, out var orderConnections))
                return;

            var tasks = orderConnections.Values.Select(conn =>
            {
                try
                {
                    return conn.SendMessageAsync(message);
                }
                catch (Exception ex)
                {
                    conn._logger.LogError(ex, "广播消息异常: Order={Order}, CID={Cid}", conn._payOrderId, conn._cid);
                    return Task.CompletedTask;
                }
            });
            LogUtil<WsPayOrderServer>.Info($"已广播消息到 {tasks.Count()} 个客户端: Order={payOrderId}");

            await Task.WhenAll(tasks);
        }
        #endregion
    }
}