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

        public Task ReceiveAsync(ResetIsvAgentMchAppInfoConfigMQ.MsgPayload payload)
        {
            _logger.LogInformation("接收商户通知MQ, 消息: {payload}", JsonConvert.SerializeObject(payload));
            //_logger.LogInformation($"接收商户通知MQ, 消息: {JsonConvert.SerializeObject(payload)}");
            if (payload.ResetType == ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO)
            {
                return this.ModifyIsvInfoAsync(payload.IsvNo);
            }
            else if (payload.ResetType == ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_AGENT_INFO)
            {
                return this.ModifyAgentInfoAsync(payload.MchNo);
            }
            else if (payload.ResetType == ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_INFO)
            {
                return this.ModifyMchInfoAsync(payload.MchNo);
            }
            else if (payload.ResetType == ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_APP)
            {
                return this.ModifyMchAppAsync(payload.MchNo, payload.AppId);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 接收 [商户配置信息] 的消息
        /// </summary>
        /// <param name="mchNo"></param>
        private async Task ModifyMchInfoAsync(string mchNo)
        {
            _logger.LogInformation("成功接收 [商户配置信息] 的消息, 消息: {mchNo}", mchNo);
            //_logger.LogInformation($"成功接收 [商户配置信息] 的消息, 消息: {mchNo}");
            await _configContextService.InitMchInfoConfigContextAsync(mchNo);
            _logger.LogInformation(" [商户配置信息] 已重置");
        }

        /// <summary>
        /// 接收 [商户应用支付参数配置信息] 的消息
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="appId"></param>
        private async Task ModifyMchAppAsync(string mchNo, string appId)
        {
            _logger.LogInformation("成功接收 [商户应用支付参数配置信息] 的消息, mchNo={mchNo}, appId={appId}", mchNo, appId);
            //_logger.LogInformation($"成功接收 [商户应用支付参数配置信息] 的消息, mchNo={mchNo}, appId={appId}");
            await _configContextService.InitMchAppConfigContextAsync(mchNo, appId);
            _logger.LogInformation(" [商户应用支付参数配置信息] 已重置");
        }

        /// <summary>
        /// 重置代理商信息
        /// </summary>
        /// <param name="agentNo"></param>
        private async Task ModifyAgentInfoAsync(string agentNo)
        {
            _logger.LogInformation("成功接收 [代理商信息] 重置, 消息: {agentNo}", agentNo);
            //_logger.LogInformation($"成功接收 [代理商信息] 重置, 消息: {agentNo}");
            await _configContextService.InitAgentConfigContextAsync(agentNo);
            _logger.LogInformation("[代理商信息] 已重置");
        }

        /// <summary>
        /// 重置ISV信息
        /// </summary>
        /// <param name="isvNo"></param>
        private async Task ModifyIsvInfoAsync(string isvNo)
        {
            _logger.LogInformation("成功接收 [ISV信息] 重置, 消息: {isvNo}", isvNo);
            //_logger.LogInformation($"成功接收 [ISV信息] 重置, 消息: {isvNo}");
            await _configContextService.InitIsvConfigContextAsync(isvNo);
            _logger.LogInformation("[ISV信息] 已重置");
        }
    }
}
