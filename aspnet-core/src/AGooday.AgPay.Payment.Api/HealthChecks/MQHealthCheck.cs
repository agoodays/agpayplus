using AGooday.AgPay.Components.MQ.Vender;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AGooday.AgPay.Payment.Api.HealthChecks
{
    /// <summary>
    /// MQ 健康检查（针对 RabbitMQ）
    /// 检查能否建立短连接到 Broker
    /// </summary>
    public class MQHealthCheck : IHealthCheck
    {
        private readonly IMQSender _mqSender;
        private readonly ILogger<MQHealthCheck> _logger;

        public MQHealthCheck(IMQSender mqSender, ILogger<MQHealthCheck> logger)
        {
            _mqSender = mqSender;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var ok = await _mqSender.CheckHealthAsync(cancellationToken);
                if (ok)
                {
                    return HealthCheckResult.Healthy("MQ 可用");
                }

                return HealthCheckResult.Unhealthy("MQ 不可用");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQ 健康检查异常");
                return HealthCheckResult.Unhealthy("MQ 健康检查异常", ex);
            }
        }
    }
}
