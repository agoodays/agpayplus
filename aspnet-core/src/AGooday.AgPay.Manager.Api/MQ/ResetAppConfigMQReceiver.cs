using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.MQ.Models;
using Newtonsoft.Json;

namespace AGooday.AgPay.Manager.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：更新系统配置参数
    /// </summary>
    public class ResetAppConfigMQReceiver : ResetAppConfigMQ.IMQReceiver
    {
        private readonly ILogger<ResetAppConfigMQReceiver> log;
        private readonly IServiceProvider provider;

        public ResetAppConfigMQReceiver(ILogger<ResetAppConfigMQReceiver> log, IServiceProvider provider)
        {
            this.log = log;
            this.provider = provider;
        }

        public void Receive(ResetAppConfigMQ.MsgPayload payload)
        {
            using (var scope = provider.CreateScope())
            {
                var sysConfigService = scope.ServiceProvider.GetRequiredService<ISysConfigService>();
                log.LogInformation($"成功接收更新系统配置的订阅通知, msg={JsonConvert.SerializeObject(payload)}");
                sysConfigService.InitDBConfig(payload.GroupKey);
                log.LogInformation("系统配置静态属性已重置");
            }
        }
    }
}
