using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel;
using System.Reflection;

namespace AGooday.AgPay.Payment.Api.Utils
{
    /// <summary>
    /// 支付方式动态调用Utils
    /// </summary>
    public class PayWayUtil
    {
        private static readonly string PAYWAY_PACKAGE_NAME = "PayWay";
        private static readonly string PAYWAYV3_PACKAGE_NAME = "PayWayV3";

        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 获取真实的支付方式Service
        /// </summary>
        /// <param name="service"></param>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static IPaymentService GetRealPayWayService(IPaymentService service, string wayCode)
        {
            return GetRealPayWayService(service, PAYWAY_PACKAGE_NAME, wayCode);
        }

        /// <summary>
        /// 获取微信V3真实的支付方式Service
        /// </summary>
        /// <param name="service"></param>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static IPaymentService GetRealPayWayV3Service(IPaymentService service, string wayCode)
        {
            return GetRealPayWayService(service, PAYWAYV3_PACKAGE_NAME, wayCode);
        }

        public static void PayWayServiceRegister<T>(IServiceCollection services) where T : AbstractPaymentService
        {
            PayWayServiceRegister<T>(services, PAYWAY_PACKAGE_NAME);
        }

        public static void PayWayV3ServiceRegister<T>(IServiceCollection services) where T : AbstractPaymentService
        {
            PayWayServiceRegister<T>(services, PAYWAYV3_PACKAGE_NAME);
        }

        private static void PayWayServiceRegister<T>(IServiceCollection services, string packageName) where T : AbstractPaymentService
        {
            string targetNamespace = $"{typeof(T).Namespace}.{packageName}";

            Assembly assembly = Assembly.GetExecutingAssembly();

            // 获取指定命名空间下的所有类
            Type[] targetTypes = assembly.GetTypes()
                // 根据命名空间和实现了 IPaymentService 接口的类型进行筛选，typeof(IPaymentService).IsAssignableFrom(type) 来筛选实现了 IPaymentService 接口的类型
                .Where(type => type.Namespace == targetNamespace && typeof(IPaymentService).IsAssignableFrom(type))
                .ToArray();

            // 注册所有类
            foreach (Type type in targetTypes)
            {
                services.AddKeyedScoped(typeof(IPaymentService), GetServiceKey(targetNamespace, type.Name), type);
            }
        }

        private static string GetServiceKey(string targetNamespace, string wayCode)
        {
            return $"{targetNamespace}.{wayCode}".ToLower();
        }

        private static IPaymentService GetRealPayWayService(IPaymentService service, string packageName, string wayCode)
        {
            try
            {
                string targetNamespace = $"{service.GetType().Namespace}.{packageName}";
                //var serviceName = $"{service.GetType().Namespace}.{packageName}.{RenameUtil.SnakeCaseToUpperCamelCase(wayCode.ToLower())}";
                //IPaymentService paymentService = ServiceProvider.GetServices<IPaymentService>()
                //    .FirstOrDefault(f => $"{f.GetType().Namespace}.{f.GetType().Name}".Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                var serviceKey = GetServiceKey(targetNamespace, RenameUtil.SnakeCaseToUpperCamelCase(wayCode));
                var paymentService = ServiceProvider.GetRequiredKeyedService<IPaymentService>(serviceKey);
                if (paymentService == null)
                {
                    throw new BizException("支付接口不支持该支付方式");
                }
                return paymentService;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
