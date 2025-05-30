using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace AGooday.AgPay.Merchant.Api.WebSockets
{
    /// <summary>
    /// 通用WebSocket服务器框架
    /// </summary>
    public abstract class AbstractWebSocketServer<TKey>
    {
        protected readonly ILogger<AbstractWebSocketServer<TKey>> _logger;

        // appId 与 WsChannelUserIdServer 存储关系, ConcurrentHashMap保证线程安全
        // REF: https://radu-matei.com/blog/aspnet-core-websockets-middleware/
        // 连接池：外层字典为业务分组，内层字典为具体连接
        protected readonly ConcurrentDictionary<TKey, ConcurrentDictionary<string, WebSocket>> _connectionGroups = new();

        protected AbstractWebSocketServer(ILogger<AbstractWebSocketServer<TKey>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 处理WebSocket连接
        /// </summary>
        public virtual async Task ProcessWebSocketAsync(WebSocket webSocket, string connectionId, TKey groupKey)
        {
            try
            {
                // 注册连接
                RegisterConnection(groupKey, connectionId, webSocket);

                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result;

                // 保持连接活跃
                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await HandleCloseRequestAsync(webSocket, result);
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        _logger.LogInformation("收到消息: {GroupKey}, {ConnectionId}, {Message}",
                            groupKey, connectionId, message);
                    }
                }
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                _logger.LogWarning("客户端异常断开: {ConnectionId}", connectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket处理异常: {ConnectionId}", connectionId);
            }
            finally
            {
                // 清理资源
                await CleanupResourcesAsync(groupKey, connectionId, webSocket);
            }
        }

        /// <summary>
        /// 注册连接
        /// </summary>
        protected virtual void RegisterConnection(TKey groupKey, string connectionId, WebSocket webSocket)
        {
            var group = _connectionGroups.GetOrAdd(groupKey, _ => new ConcurrentDictionary<string, WebSocket>());
            if (!group.TryAdd(connectionId, webSocket))
            {
                _logger.LogWarning("连接已存在: {GroupKey}, {ConnectionId}", groupKey, connectionId);
                return;
            }

            _logger.LogInformation("新连接注册: {GroupKey}, {ConnectionId}, 当前连接数: {Count}",
                groupKey, connectionId, group.Count);
        }

        /// <summary>
        /// 处理关闭请求
        /// </summary>
        protected virtual async Task HandleCloseRequestAsync(WebSocket webSocket, WebSocketReceiveResult result)
        {
            try
            {
                await webSocket.CloseAsync(
                    result.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                    result.CloseStatusDescription,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogDebug("关闭握手异常: {Message}", ex.Message);
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        protected virtual async Task CleanupResourcesAsync(TKey groupKey, string connectionId, WebSocket webSocket)
        {
            try
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "服务端关闭",
                        CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("关闭连接时异常: {Message}", ex.Message);
            }
            finally
            {
                webSocket.Dispose();
                RemoveConnection(groupKey, connectionId);
            }
        }

        /// <summary>
        /// 移除连接
        /// </summary>
        protected virtual void RemoveConnection(TKey groupKey, string connectionId)
        {
            if (!_connectionGroups.TryGetValue(groupKey, out var group)) return;

            if (!group.TryRemove(connectionId, out _)) return;

            _logger.LogInformation("连接已移除: {GroupKey}, {ConnectionId}, 剩余连接数: {Count}",
                groupKey, connectionId, group.Count);

            if (group.IsEmpty)
            {
                _connectionGroups.TryRemove(groupKey, out _);
                _logger.LogInformation("业务组已清空: {GroupKey}", groupKey);
            }
        }

        /// <summary>
        /// 广播消息到业务组
        /// </summary>
        public virtual async Task BroadcastToGroupAsync(TKey groupKey, string message)
        {
            if (!_connectionGroups.TryGetValue(groupKey, out var group)) return;

            var tasks = new List<Task>();
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            foreach (var (connectionId, webSocket) in group)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    tasks.Add(SendMessageAsync(webSocket, connectionId, segment));
                }
                else
                {
                    // 清理无效连接
                    RemoveConnection(groupKey, connectionId);
                }
            }

            await Task.WhenAll(tasks);
            _logger.LogInformation("已广播消息到 {Count} 个客户端: {GroupKey}", tasks.Count, groupKey);
        }

        /// <summary>
        /// 发送消息到单个连接
        /// </summary>
        protected virtual async Task SendMessageAsync(WebSocket webSocket, string connectionId, ArraySegment<byte> buffer)
        {
            try
            {
                await webSocket.SendAsync(
                    buffer,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "消息发送失败: {ConnectionId}", connectionId);
            }
        }
    }
}