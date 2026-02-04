using AGooday.AgPay.Base.Api.Extensions;

namespace AGooday.AgPay.Payment.Api.Extensions
{
    /// <summary>
    /// Payment.Api 健康检查配置
    /// </summary>
    public static class HealthChecksExtensions
    {
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
        {
            // 使用 Base.Api 提供的基础健康检查（Redis + MQ）
            // 并添加 Payment.Api 特有的 Quartz 健康检查
            services.AddBaseHealthChecks()
                .AddQuartzHealthCheck();
            return services;
        }
    }
}
