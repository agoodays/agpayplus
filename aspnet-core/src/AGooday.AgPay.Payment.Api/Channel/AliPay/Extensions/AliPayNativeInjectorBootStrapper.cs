using System.Reflection;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.Extensions
{
    public class AliPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //services.AddScoped<IPaymentService, AliApp>();
            //services.AddScoped<IPaymentService, AliBar>();
            //services.AddScoped<IPaymentService, AliJsapi>();
            //services.AddScoped<IPaymentService, AliLite>();
            //services.AddScoped<IPaymentService, AliPc>();
            //services.AddScoped<IPaymentService, AliQr>();
            //services.AddScoped<IPaymentService, AliWap>();

            string targetNamespace = typeof(AliPayNativeInjectorBootStrapper).Namespace.Replace(".Extensions", ".PayWay");

            Assembly assembly = Assembly.GetExecutingAssembly();

            // 获取指定命名空间下的所有类
            Type[] targetTypes = assembly.GetTypes()
                .Where(type => type.Namespace == targetNamespace)
                .ToArray();

            // 注册所有类
            foreach (Type type in targetTypes)
            {
                services.AddScoped(typeof(IPaymentService), type);
            }
        }
    }
}
