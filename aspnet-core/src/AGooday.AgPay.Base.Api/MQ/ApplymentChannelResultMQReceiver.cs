using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.MQ.Models;
using Newtonsoft.Json;

namespace AGooday.AgPay.Base.Api.MQ
{
    /// <summary>
    /// 商户进件渠道结果 MQ 消费者
    ///
    /// 负责将通道查询 / 签约等异步场景的结果写回 DB。
    /// 实现 ApplymentChannelResultMQ.IMQReceiver 接口，
    /// RabbitMQ Receiver 会自动扫描到该实现并完成路由。
    ///
    /// 处理流程：
    ///   收到 MsgPayload → 创建 Scope（因为本类是 Singleton，IMchApplyService 是 Scoped）
    ///   → 调用 IMchApplyService.PatchChannelResultAsync 更新 DB
    ///   → 日志记录
    /// </summary>
    public class ApplymentChannelResultMQReceiver : ApplymentChannelResultMQ.IMQReceiver
    {
        private readonly ILogger<ApplymentChannelResultMQReceiver> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ApplymentChannelResultMQReceiver(
            ILogger<ApplymentChannelResultMQReceiver> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ReceiveAsync(ApplymentChannelResultMQ.MsgPayload payload)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mchApplyService = scope.ServiceProvider.GetService<IMchApplyService>();
                try
                {
                    _logger.LogInformation(
                        "收到进件渠道结果MQ消息 | ApplyId={ApplyId} State={State} Progress={Progress}",
                        payload.ApplyId, payload.State, payload.Progress);

                    var patch = new MchApplyChannelResultPatch
                    {
                        State = payload.State,
                        Progress = payload.Progress,
                        ChannelApplyNo = payload.ChannelApplyNo,
                        ChannelMchId = payload.ChannelMchId,
                        ApplyErrorInfo = payload.ApplyErrorInfo,
                        SuccResParameter = payload.SuccResParameter,
                        SignUrl = payload.SignUrl,
                        SignExpireAt = payload.SignExpireAt,
                        VerifyAmount = payload.VerifyAmount,
                    };

                    var ok = await mchApplyService.PatchChannelResultAsync(payload.ApplyId, patch);

                    if (ok)
                    {
                        var stateDesc = payload.State.HasValue
                            ? ((ApplymentState)payload.State.Value).GetDescription()
                            : "(unchanged)";
                        _logger.LogInformation(
                            "进件[{ApplyId}] DB更新成功 → State={StateDesc} Progress={Progress}",
                            payload.ApplyId, stateDesc, payload.Progress);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "进件[{ApplyId}] 更新失败：记录不存在或DB异常", payload.ApplyId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "进件[{ApplyId}] MQ消费异常 | 消息: {Payload}",
                        payload.ApplyId, JsonConvert.SerializeObject(payload));
                }
            }
        }
    }
}
