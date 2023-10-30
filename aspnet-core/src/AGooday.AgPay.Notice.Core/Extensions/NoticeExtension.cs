using AGooday.AgPay.Notice.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NoticeExtension
    {
        public static IServiceCollection AddNotice(this IServiceCollection services, Action<NoticeOptions> configure)
        {
            var options = new NoticeOptions();
            configure?.Invoke(options);

            services.AddOptions();
            services.AddOptions<NoticeOptions>().Configure(configure);

            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }
            return services;
        }
    }
}
