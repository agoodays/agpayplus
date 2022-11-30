using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel;

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
            try
            {
                var serviceName = $"{service.GetType().Namespace}.{PAYWAY_PACKAGE_NAME}.{RenameUtil.SnakeCaseToUpperCamelCase(wayCode.ToLower())}";
                IPaymentService paymentService = ServiceProvider.GetServices<IPaymentService>()
                    .FirstOrDefault(f => $"{f.GetType().Namespace}.{f.GetType().Name}".Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                return paymentService;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取微信V3真实的支付方式Service
        /// </summary>
        /// <param name="service"></param>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static IPaymentService GetRealPayWayV3Service(IPaymentService service, string wayCode)
        {
            try
            {
                var serviceName = $"{service.GetType().Namespace}.{PAYWAYV3_PACKAGE_NAME}.{RenameUtil.SnakeCaseToUpperCamelCase(wayCode.ToLower())}";
                IPaymentService paymentService = ServiceProvider.GetServices<IPaymentService>()
                    .FirstOrDefault(f => $"{f.GetType().Namespace}.{f.GetType().Name}".Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                return paymentService;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
