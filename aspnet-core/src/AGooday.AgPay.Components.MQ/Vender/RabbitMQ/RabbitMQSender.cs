using System.Collections.Concurrent;
using System.Text;
using AGooday.AgPay.Components.MQ.Constants;
using AGooday.AgPay.Components.MQ.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ
{
    /// <summary>
    /// https://github.com/whuanle/learnrabbitmq
    /// https://www.rabbitmq.com/client-libraries/dotnet-api-guide
    /// RabbitMQ 消息发送器实现
    /// </summary>
    public class RabbitMQSender : IMQSender, IDisposable
    {
        // 配置对象
        private readonly RabbitMQConfig _config;
        private readonly ILogger<RabbitMQSender> _logger;
        private readonly IServiceProvider _serviceProvider;

        // 连接和通道
        private IConnection _sendConnection;
        private IChannel _sendChannel;
        private IConnection _receiveConnection;
        private readonly ConcurrentBag<IChannel> _receiveChannels = new();

        // 消息处理任务跟踪
        private readonly ConcurrentBag<Task> _pendingTasks = new();
        private readonly CancellationTokenSource _cts = new();

        public RabbitMQSender(
            IOptions<RabbitMQConfig> config,
            ILogger<RabbitMQSender> logger,
            IServiceProvider serviceProvider)
        {
            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            // 不在构造函数中直接初始化连接，延迟到首次发送时
        }

        private async Task EnsureSendInfrastructureAsync()
        {
            if (_sendConnection != null && _sendChannel != null && _sendConnection.IsOpen && _sendChannel.IsOpen)
                return;

            var factory = new ConnectionFactory
            {
                HostName = _config.MQ.HostName,
                UserName = _config.MQ.UserName,
                Password = _config.MQ.Password,
                Port = _config.MQ.Port.Value,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            };

            _sendConnection = await factory.CreateConnectionAsync();
            _sendChannel = await _sendConnection.CreateChannelAsync();
            _logger.LogInformation("RabbitMQ 发送基础设施初始化完成");
        }

        public async Task SendAsync(AbstractMQ mqModel)
        {
            await EnsureSendInfrastructureAsync();
            if (mqModel.GetMQType() == MQSendTypeEnum.QUEUE)
            {
                await ConvertAndSendAsync(mqModel.GetMQType(), "", mqModel.GetMQName(), mqModel.GetMQName(), mqModel.ToMessage());
            }
            else
            {
                // fanout模式 的 routeKEY 没意义。
                await ConvertAndSendAsync(
                    mqModel.GetMQType(),
                    RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX + mqModel.GetMQName(),
                    "",
                    "",
                    mqModel.ToMessage());
            }
        }

        public async Task SendAsync(AbstractMQ mqModel, int delay)
        {
            await EnsureSendInfrastructureAsync();
            if (mqModel.GetMQType() == MQSendTypeEnum.QUEUE)
            {
                var queue = mqModel.GetMQName();
                await ConvertAndDelaySendAsync(RabbitMQConfig.DELAYED_EXCHANGE_NAME, queue, "delay.delay", mqModel.ToMessage(), delay);
            }
            else
            {
                // fanout模式 的 routeKEY 没意义。
                await SendAsync(mqModel); // 广播模式不支持延迟
            }
        }

        public async Task ReceiveAsync()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _config.MQ.HostName,
                    UserName = _config.MQ.UserName,
                    Password = _config.MQ.Password,
                    Port = _config.MQ.Port.Value,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                };

                _receiveConnection = await factory.CreateConnectionAsync();

                // 获取所有带 RabbitMQReceiverAttribute 的接收器
                var msgReceivers = _serviceProvider.GetServices<IMQMsgReceiver>()
                    .Where(w => Attribute.IsDefined(w.GetType(), typeof(RabbitMQReceiverAttribute)));

                foreach (var msgReceiver in msgReceivers)
                {
                    var channel = await _receiveConnection.CreateChannelAsync();
                    _receiveChannels.Add(channel);

                    string queueName;
                    if (msgReceiver.GetMQType() == MQSendTypeEnum.QUEUE)
                    {
                        queueName = msgReceiver.GetMQName();
                        await channel.QueueDeclareAsync(
                            queue: queueName,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
                    }
                    else
                    {
                        var exchange = RabbitMQConfig.FANOUT_EXCHANGE_NAME_PREFIX + msgReceiver.GetMQName();
                        await channel.ExchangeDeclareAsync(exchange, "fanout", true);
                        var queue = await channel.QueueDeclareAsync();
                        queueName = queue.QueueName;
                        await channel.QueueBindAsync(queueName, exchange, "");
                    }

                    // 配置QoS（预取数量）
                    await channel.BasicQosAsync(0, _config.MQ.PrefetchCount.Value, false);

                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);

                            // 创建独立作用域处理消息
                            using var scope = _serviceProvider.CreateScope();
                            var receiver = scope.ServiceProvider.GetRequiredService(msgReceiver.GetType()) as IMQMsgReceiver;

                            await receiver.ReceiveMsgAsync(message);
                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "消息处理失败");
                            await channel.BasicNackAsync(ea.DeliveryTag, false, true); // 重新入队
                        }
                    };

                    consumer.ShutdownAsync += (sender, ea) =>
                    {
                        _logger.LogWarning($"消费者关闭: {ea.ReplyText}");
                        return Task.CompletedTask;
                    };

                    await channel.BasicConsumeAsync(queueName, false, consumer);
                    _logger.LogInformation($"启动消费者: {msgReceiver.GetType().Name}, 队列: {queueName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ消息接收初始化失败");
                throw;
            }
        }

        public async Task CloseAsync()
        {
            _cts.Cancel(); // 取消所有待处理任务

            // 等待所有消息处理完成（最多30秒）
            await Task.WhenAll(_pendingTasks.ToArray()).WaitAsync(TimeSpan.FromSeconds(30));

            // 关闭接收通道
            foreach (var channel in _receiveChannels)
            {
                if (channel.IsOpen)
                    await channel.CloseAsync();
            }

            // 关闭发送通道
            if (_sendChannel?.IsOpen == true)
                await _sendChannel.CloseAsync();

            // 关闭连接
            if (_receiveConnection?.IsOpen == true)
                await _receiveConnection.CloseAsync();

            if (_sendConnection?.IsOpen == true)
                await _sendConnection.CloseAsync();

            _logger.LogInformation("RabbitMQ 连接已关闭");
        }

        private async Task ConvertAndSendAsync(
            MQSendTypeEnum mqtype,
            string exchange,
            string queue,
            string routingKey,
            string message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties
                {
                    Persistent = true, // 持久化消息
                    Headers = new Dictionary<string, object>
                    {
                        { "AppId", "AgPay" },
                        { "Timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }
                    }
                };

                if (mqtype == MQSendTypeEnum.BROADCAST && !string.IsNullOrWhiteSpace(exchange))
                {
                    await _sendChannel.ExchangeDeclareAsync(
                        exchange: exchange,
                        type: "fanout",
                        durable: true);
                }

                await _sendChannel.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);

                _logger.LogDebug($"消息发送成功: {message.Substring(0, Math.Min(100, message.Length))}...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RabbitMQ消息推送失败: {message}");
                throw;
            }
        }

        private async Task ConvertAndDelaySendAsync(
            string exchange,
            string queue,
            string routingKey,
            string message,
            int delay)
        {
            try
            {
                // 声明延迟交换机和队列
                var arguments = new Dictionary<string, object> { { "x-delayed-type", "topic" } };
                //设置当前消息为延时队列, 需要安装延时插件: https://www.yuque.com/xiangyisheng/kgcg9t/vmhkyo
                await _sendChannel.ExchangeDeclareAsync(
                    exchange: exchange,
                    type: "x-delayed-message",
                    durable: true,
                    autoDelete: false,
                    arguments: arguments);

                await _sendChannel.QueueDeclareAsync(
                    queue: queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments);

                await _sendChannel.QueueBindAsync(queue, exchange, routingKey);

                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    Headers = new Dictionary<string, object>
                    {
                        { "x-delay", delay * 1000 }, // 延迟时间，单位毫秒
                        { "AppId", "AgPay" },
                        { "Timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }
                    }
                };

                await _sendChannel.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);

                _logger.LogDebug($"延迟消息发送成功: {delay}ms, {message.Substring(0, Math.Min(100, message.Length))}...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RabbitMQ延迟消息推送失败: {message}");
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                CloseAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQSender Dispose 异常");
            }
            _cts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
