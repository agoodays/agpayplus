using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务： 更新服务商/商户/商户应用配置信息；
    /// </summary>
    public class ResetIsvMchAppInfoMQReceiver : ResetIsvMchAppInfoConfigMQ.IMQReceiver
    {
        private readonly ILogger<ResetIsvMchAppInfoMQReceiver> log;
        private readonly ConfigContextService configContextService;

        public ResetIsvMchAppInfoMQReceiver(ILogger<ResetIsvMchAppInfoMQReceiver> log)
        {
            this.log = log;
        }

        public ResetIsvMchAppInfoMQReceiver(ConfigContextService configContextService)
        {
            this.configContextService = configContextService;
        }

        public void Receive(ResetIsvMchAppInfoConfigMQ.MsgPayload payload)
        {
            log.LogInformation($"接收商户通知MQ, msg={JsonConvert.SerializeObject(payload)}");
            if (payload.ResetType == ResetIsvMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO)
            {
                this.ModifyIsvInfo(payload.IsvNo);
            }
            else if (payload.ResetType == ResetIsvMchAppInfoConfigMQ.RESET_TYPE_MCH_INFO)
            {
                this.ModifyMchInfo(payload.MchNo);
            }
            else if (payload.ResetType == ResetIsvMchAppInfoConfigMQ.RESET_TYPE_MCH_APP)
            {
                this.ModifyMchApp(payload.MchNo, payload.AppId);
            }
        }

        /// <summary>
        /// 接收 [商户配置信息] 的消息
        /// </summary>
        /// <param name="mchNo"></param>
        private void ModifyMchInfo(String mchNo)
        {
            log.LogInformation("成功接收 [商户配置信息] 的消息, msg={}", mchNo);
            configContextService.InitMchInfoConfigContext(mchNo);
            log.LogInformation(" [商户配置信息] 已重置");
        }

        /** 接收 [商户应用支付参数配置信息] 的消息 **/
        private void ModifyMchApp(String mchNo, String appId)
        {
            log.LogInformation("成功接收 [商户应用支付参数配置信息] 的消息, mchNo={}, appId={}", mchNo, appId);
            configContextService.InitMchAppConfigContext(mchNo, appId);
            log.LogInformation(" [商户应用支付参数配置信息] 已重置");
        }

        /** 重置ISV信息 **/
        private void ModifyIsvInfo(String isvNo)
        {
            log.LogInformation("成功接收 [ISV信息] 重置, msg={}", isvNo);
            configContextService.InitIsvConfigContext(isvNo);
            log.LogInformation("[ISV信息] 已重置");
        }
    }
}
