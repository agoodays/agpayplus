using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;

namespace AGooday.AgPay.Base.Api.HealthChecks
{
    /// <summary>
    /// Quartz 调度器健康检查
    /// </summary>
    public class QuartzHealthCheck : IHealthCheck
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<QuartzHealthCheck> _logger;

        public QuartzHealthCheck(ISchedulerFactory schedulerFactory, ILogger<QuartzHealthCheck> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

                if (!scheduler.IsStarted)
                {
                    return HealthCheckResult.Unhealthy("Quartz 调度器未启动");
                }

                if (scheduler.InStandbyMode)
                {
                    return HealthCheckResult.Degraded("Quartz 调度器处于待机模式");
                }

                // 获取当前执行的任务数量
                var executingJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);

                var data = new Dictionary<string, object>
                {
                    { "IsStarted", scheduler.IsStarted },
                    { "SchedulerName", scheduler.SchedulerName },
                    { "SchedulerInstanceId", scheduler.SchedulerInstanceId },
                    { "ExecutingJobsCount", executingJobs.Count }
                };

                return HealthCheckResult.Healthy("Quartz 调度器运行正常", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Quartz 健康检查失败");
                return HealthCheckResult.Unhealthy("Quartz 调度器异常", ex);
            }
        }
    }
}
