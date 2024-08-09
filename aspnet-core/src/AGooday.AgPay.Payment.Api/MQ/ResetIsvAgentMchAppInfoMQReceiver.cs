using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：更新服务商/代理商/商户/商户应用配置信息；
    /// </summary>
    public class ResetIsvAgentMchAppInfoMQReceiver : ResetIsvAgentMchAppInfoConfigMQ.IMQReceiver
    {
        private readonly ILogger<ResetIsvAgentMchAppInfoMQReceiver> _logger;
        private readonly ConfigContextService _configContextService;

        public ResetIsvAgentMchAppInfoMQReceiver(ILogger<ResetIsvAgentMchAppInfoMQReceiver> logger,
            ConfigContextService configContextService)
        {
            _logger = logger;
            _configContextService = configContextService;
        }

        public void Receive(ResetIsvAgentMchAppInfoConfigMQ.MsgPayload payload)
        {
            _logger.LogInformation($"接收商户通知MQ, msg={JsonConvert.SerializeObject(payload)}");
            if (payload.ResetType == ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO)
            {
                this.ModifyIsvInfo(payload.IsvNo);
            }
            else if (payload.ResetType == ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_AGENT_INFO)
            {
                this.ModifyAgentInfo(payload.MchNo);
            }
            else if (payload.ResetType == ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_INFO)
            {
                this.ModifyMchInfo(payload.MchNo);
            }
            else if (payload.ResetType == ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_APP)
            {
                this.ModifyMchApp(payload.MchNo, payload.AppId);
            }
        }

        /// <summary>
        /// 接收 [商户配置信息] 的消息
        /// </summary>
        /// <param name="mchNo"></param>
        private void ModifyMchInfo(string mchNo)
        {
            _logger.LogInformation($"成功接收 [商户配置信息] 的消息, msg={mchNo}");
            _configContextService.InitMchInfoConfigContext(mchNo);
            _logger.LogInformation(" [商户配置信息] 已重置");
        }

        /// <summary>
        /// 接收 [商户应用支付参数配置信息] 的消息
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="appId"></param>
        private void ModifyMchApp(string mchNo, string appId)
        {
            _logger.LogInformation($"成功接收 [商户应用支付参数配置信息] 的消息, mchNo={mchNo}, appId={appId}");
            _configContextService.InitMchAppConfigContext(mchNo, appId);
            _logger.LogInformation(" [商户应用支付参数配置信息] 已重置");
        }

        /// <summary>
        /// 重置代理商信息
        /// </summary>
        /// <param name="agentNo"></param>
        private void ModifyAgentInfo(string agentNo)
        {
            _logger.LogInformation($"成功接收 [代理商信息] 重置, msg={agentNo}");
            _configContextService.InitAgentConfigContext(agentNo);
            _logger.LogInformation("[代理商信息] 已重置");
        }

        /// <summary>
        /// 重置ISV信息
        /// </summary>
        /// <param name="isvNo"></param>
        private void ModifyIsvInfo(string isvNo)
        {
            _logger.LogInformation($"成功接收 [ISV信息] 重置, msg={isvNo}");
            _configContextService.InitIsvConfigContext(isvNo);
            _logger.LogInformation("[ISV信息] 已重置");
        }
    }
}
