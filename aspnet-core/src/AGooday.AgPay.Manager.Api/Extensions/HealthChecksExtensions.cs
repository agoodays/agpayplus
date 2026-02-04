using AGooday.AgPay.Base.Api.Extensions;

namespace AGooday.AgPay.Manager.Api.Extensions
{
    /// <summary>
    /// Manager.Api 健康检查配置
    /// </summary>
    public static class HealthChecksExtensions
    {
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
        {
            // 使用 Base.Api 提供的基础健康检查（Redis + MQ）
            services.AddBaseHealthChecks();
            return services;
        }
    }
}
