using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.MQ.Models;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：更新系统配置参数
    /// </summary>
    public class ResetAppConfigMQReceiver : ResetAppConfigMQ.IMQReceiver
    {
        private readonly ILogger<ResetAppConfigMQReceiver> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ISysConfigService sysConfigService;

        public ResetAppConfigMQReceiver(ILogger<ResetAppConfigMQReceiver> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Receive(ResetAppConfigMQ.MsgPayload payload)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                this.sysConfigService = scope.ServiceProvider.GetService<ISysConfigService>();
                _logger.LogInformation($"成功接收更新系统配置的订阅通知, msg={JsonConvert.SerializeObject(payload)}");
                sysConfigService.InitDBConfig(payload.GroupKey);
                _logger.LogInformation("系统配置静态属性已重置");
            }
        }
    }
}
