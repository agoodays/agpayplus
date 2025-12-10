using AGooday.AgPay.Components.MQ.Vender;

namespace AGooday.AgPay.Manager.Api.MQ
{
    public class MQReceiverHostedService : IHostedService
    {
        private readonly ILogger<MQReceiverHostedService> _logger;
        private readonly IMQSender _mqSender;

        public MQReceiverHostedService(ILogger<MQReceiverHostedService> logger, IMQSender mqSender)
        {
            _logger = logger;
            _mqSender = mqSender;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("启动MQ消息接收服务...");
            //// 启动后台任务，在后台接收消息
            //return _mqSender.ReceiveAsync();
            _logger.LogInformation("启动MQ消息接收服务...");
            try
            {
                // 等待初始化完成（连接、声明队列等）
                await _mqSender.ReceiveAsync();
                _logger.LogInformation("MQ消息接收服务启动成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQ消息接收服务启动失败");
                // 可选：让应用崩溃 or 记录后继续（取决于业务容忍度）
                //throw; // 推荐：让 HostedService 启动失败，触发健康检查告警
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("停止MQ消息接收服务...");
            //// 在停止时执行必要的清理操作
            //return _mqSender.CloseAsync();
            _logger.LogInformation("停止MQ消息接收服务...");
            try
            {
                await _mqSender.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止MQ消息接收服务时发生错误");
            }
        }
    }
}
