namespace AGooday.AgPay.Components.Third.Models
{
    public static class ServiceResolver
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public static T GetService<T>() where T : class => _serviceProvider.GetService<T>();

        public static T GetRequiredKeyedService<T>(object serviceKey) where T : notnull => _serviceProvider.GetRequiredKeyedService<T>(serviceKey);
    }
}
