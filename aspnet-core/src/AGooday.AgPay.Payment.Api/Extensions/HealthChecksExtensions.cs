using AGooday.AgPay.Payment.Api.HealthChecks;

namespace AGooday.AgPay.Payment.Api.Extensions
{
    public static class HealthChecksExtensions
    {
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<RedisHealthCheck>("redis", tags: new[] { "ready", "live" })
                .AddCheck<MQHealthCheck>("rabbitmq", tags: new[] { "ready" })
                .AddCheck<QuartzHealthCheck>("quartz", tags: new[] { "ready" });
            return services;
        }
    }
}
