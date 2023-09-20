using AGooday.AgPay.Payment.Api.Channel.YsfPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay.Extensions
{
    public class YsfPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPaymentService, AliBar>();
            services.AddScoped<IPaymentService, AliJsapi>();

            services.AddScoped<IPaymentService, WxBar>();
            services.AddScoped<IPaymentService, WxJsapi>();

            services.AddScoped<IPaymentService, YsfBar>();
            services.AddScoped<IPaymentService, YsfJsapi>();
        }
    }
}
