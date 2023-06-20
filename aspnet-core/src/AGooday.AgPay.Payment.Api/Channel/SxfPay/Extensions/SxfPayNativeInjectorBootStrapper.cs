using AGooday.AgPay.Payment.Api.Channel.SxfPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay.Extensions
{
    public class SxfPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPaymentService, AliBar>();
            services.AddSingleton<IPaymentService, AliQr>();
            services.AddSingleton<IPaymentService, AliJsapi>();

            services.AddSingleton<IPaymentService, WxBar>();
            services.AddSingleton<IPaymentService, WxNative>();
            services.AddSingleton<IPaymentService, WxJsapi>();

            services.AddSingleton<IPaymentService, YsfBar>();
            services.AddSingleton<IPaymentService, YsfJsapi>();
        }
    }
}
