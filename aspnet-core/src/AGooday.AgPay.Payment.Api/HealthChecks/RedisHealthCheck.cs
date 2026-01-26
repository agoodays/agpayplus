using AGooday.AgPay.Components.Cache.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AGooday.AgPay.Payment.Api.HealthChecks
{
    /// <summary>
    /// Redis 健康检查
    /// </summary>
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<RedisHealthCheck> _logger;

        public RedisHealthCheck(ICacheService cacheService, ILogger<RedisHealthCheck> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // 尝试写入测试数据
                var testKey = "health_check_test";
                var testValue = DateTime.UtcNow.ToString("O");

                await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromSeconds(10));
                var result = await _cacheService.GetAsync<string>(testKey);

                if (result == testValue)
                {
                    return HealthCheckResult.Healthy("Redis 连接正常，读写正常。");
                }

                return HealthCheckResult.Degraded("Redis 连接正常，但读写异常。");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis 健康检查失败");
                return HealthCheckResult.Unhealthy("Redis 连接失败", ex);
            }
        }
    }
}
