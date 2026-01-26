using AGooday.AgPay.Components.Cache.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AGooday.AgPay.Agent.Api.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<RedisHealthCheck> _logger;

        public RedisHealthCheck(ICacheService cacheService, ILogger<RedisHealthCheck> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var key = "health_check:test";
                var value = DateTimeOffset.UtcNow.ToString("O");
                await _cacheService.SetAsync(key, value, TimeSpan.FromSeconds(10));
                var result = await _cacheService.GetAsync<string>(key);
                if (result == value)
                {
                    return HealthCheckResult.Healthy("Redis 连接正常，读写正常。");
                }
                return HealthCheckResult.Degraded("Redis 读写不一致");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis 健康检查失败");
                return HealthCheckResult.Unhealthy("Redis 健康检查失败", ex);
            }
        }
    }
}
