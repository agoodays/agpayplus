using AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.Extensions
{
    public class AliPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPaymentService, AliApp>();
            services.AddSingleton<IPaymentService, AliBar>();
            services.AddSingleton<IPaymentService, AliJsapi>();
            services.AddSingleton<IPaymentService, AliPc>();
            services.AddSingleton<IPaymentService, AliQr>();
            services.AddSingleton<IPaymentService, AliWap>();
        }
    }
}
