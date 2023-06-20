using AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.Extensions
{
    public class WxPayV3NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPaymentService, WxApp>();
            services.AddSingleton<IPaymentService, WxBar>();
            services.AddSingleton<IPaymentService, WxH5>();
            services.AddSingleton<IPaymentService, WxJsapi>();
            services.AddSingleton<IPaymentService, WxLite>();
            services.AddSingleton<IPaymentService, WxNative>();
        }
    }
}
