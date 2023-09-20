using AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.Extensions
{
    public class WxPayV3NativeInjectorBootStrapper
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
