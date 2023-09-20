using AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.Extensions
{
    public class WxPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPaymentService, WxApp>();
            services.AddScoped<IPaymentService, WxBar>();
            services.AddScoped<IPaymentService, WxH5>();
            services.AddScoped<IPaymentService, WxJsapi>();
            services.AddScoped<IPaymentService, WxLite>();
            services.AddScoped<IPaymentService, WxNative>();
        }
    }
}
