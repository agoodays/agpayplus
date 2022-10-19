using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Components.MQ.Models;
using log4net;
using Newtonsoft.Json;

namespace AGooday.AgPay.Manager.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务： 更新系统配置参数
    /// </summary>
    public class ResetAppConfigMQReceiver : ResetAppConfigMQ.IMQReceiver
    {
        private readonly ILogger<ResetAppConfigMQReceiver> log;
        private ISysConfigService sysConfigService;

        public ResetAppConfigMQReceiver(ILogger<ResetAppConfigMQReceiver> log, ISysConfigService sysConfigService)
        {
            this.log = log;
            this.sysConfigService = sysConfigService;
        }

        public void Receive(ResetAppConfigMQ.MsgPayload payload)
        {
            log.LogInformation($"成功接收更新系统配置的订阅通知, msg={JsonConvert.SerializeObject(payload)}");
            sysConfigService.InitDBConfig(payload.GroupKey);
            log.LogInformation("系统配置静态属性已重置");
        }
    }
}
