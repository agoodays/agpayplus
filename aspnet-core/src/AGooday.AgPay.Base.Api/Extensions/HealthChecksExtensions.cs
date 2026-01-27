using AGooday.AgPay.Base.Api.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AGooday.AgPay.Base.Api.Extensions
{
    /// <summary>
    /// 健康检查扩展方法
    /// </summary>
    public static class HealthChecksExtensions
    {
        /// <summary>
        /// 添加基础健康检查（Redis + MQ）
        /// 所有 API 项目的标准配置
        /// </summary>
        public static IHealthChecksBuilder AddBaseHealthChecks(this IServiceCollection services)
        {
            return services.AddHealthChecks()
                .AddCheck<RedisHealthCheck>("redis", tags: new[] { "ready", "live" })
                .AddCheck<MQHealthCheck>("rabbitmq", tags: new[] { "ready" });
        }

        /// <summary>
        /// 添加 Quartz 健康检查
        /// 用于需要定时任务的项目（如 Payment.Api）
        /// </summary>
        public static IHealthChecksBuilder AddQuartzHealthCheck(this IHealthChecksBuilder builder)
        {
            return builder.AddCheck<QuartzHealthCheck>("quartz", tags: new[] { "ready" });
        }

        /// <summary>
        /// 添加自定义健康检查
        /// </summary>
        public static IHealthChecksBuilder AddCustomHealthCheck<THealthCheck>(
            this IHealthChecksBuilder builder,
            string name,
            params string[] tags)
            where THealthCheck : class, IHealthCheck
        {
            return builder.AddCheck<THealthCheck>(name, tags: tags);
        }
    }
}
