using AGooday.AgPay.Manager.Api.HealthChecks;

namespace AGooday.AgPay.Manager.Api.Extensions
{
    public static class HealthChecksExtensions
    {
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<RedisHealthCheck>("redis", tags: new[] { "ready", "live" })
                .AddCheck<MQHealthCheck>("rabbitmq", tags: new[] { "ready" });
            return services;
        }
    }
}
