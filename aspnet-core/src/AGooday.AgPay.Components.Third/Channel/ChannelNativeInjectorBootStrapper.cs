using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Channel.WxPay.Kits;
using AGooday.AgPay.Components.Third.Utils;
using System.Reflection;

namespace AGooday.AgPay.Components.Third.Channel
{
    public class ChannelNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            #region ChannelUserService
            ServiceRegister<IChannelUserService>(services);
            services.AddScoped<IChannelServiceFactory<IChannelUserService>, ChannelServiceFactory<IChannelUserService>>();
            #endregion
            #region DivisionService
            ServiceRegister<IDivisionService>(services);
            services.AddScoped<IChannelServiceFactory<IDivisionService>, ChannelServiceFactory<IDivisionService>>();
            #endregion
            #region DivisionRecordChannelNotifyService
            ServiceRegister<IDivisionRecordChannelNotifyService, AbstractDivisionRecordChannelNotifyService>(services);
            services.AddScoped<IChannelServiceFactory<IDivisionRecordChannelNotifyService>, ChannelServiceFactory<IDivisionRecordChannelNotifyService>>();
            #endregion
            #region PaymentService
            ServiceRegister<IPaymentService, AbstractPaymentService>(services, (targetType) =>
            {
                PayWayUtil.PayWayServiceRegister(services, targetType);
                PayWayUtil.PayWayV3ServiceRegister(services, targetType);
            });
            services.AddScoped<IChannelServiceFactory<IPaymentService>, ChannelServiceFactory<IPaymentService>>();
            #endregion
            #region RefundService
            ServiceRegister<IRefundService, AbstractRefundService>(services);
            services.AddScoped<IChannelServiceFactory<IRefundService>, ChannelServiceFactory<IRefundService>>();
            #endregion
            #region ChannelNoticeService
            ServiceRegister<IChannelNoticeService, AbstractChannelNoticeService>(services);
            services.AddScoped<IChannelServiceFactory<IChannelNoticeService>, ChannelServiceFactory<IChannelNoticeService>>();
            #endregion
            #region ChannelRefundNoticeService
            ServiceRegister<IChannelRefundNoticeService, AbstractChannelRefundNoticeService>(services);
            services.AddScoped<IChannelServiceFactory<IChannelRefundNoticeService>, ChannelServiceFactory<IChannelRefundNoticeService>>();
            #endregion
            #region CloseService
            ServiceRegister<IPayOrderCloseService>(services);
            services.AddScoped<IChannelServiceFactory<IPayOrderCloseService>, ChannelServiceFactory<IPayOrderCloseService>>();
            #endregion
            #region QueryService
            ServiceRegister<IPayOrderQueryService>(services);
            services.AddScoped<IChannelServiceFactory<IPayOrderQueryService>, ChannelServiceFactory<IPayOrderQueryService>>();
            #endregion
            #region TransferService
            ServiceRegister<ITransferService>(services);
            services.AddScoped<IChannelServiceFactory<ITransferService>, ChannelServiceFactory<ITransferService>>();
            #endregion
            #region TransferNoticeService
            ServiceRegister<ITransferNoticeService, AbstractTransferNoticeService>(services);
            services.AddScoped<IChannelServiceFactory<ITransferNoticeService>, ChannelServiceFactory<ITransferNoticeService>>();
            #endregion

            var serviceProvider = services.BuildServiceProvider();
            PayWayUtil.ServiceProvider = serviceProvider;
            AliPayKit.ServiceProvider = serviceProvider;
            WxPayKit.ServiceProvider = serviceProvider;

            ChannelCertConfigKit.ServiceProvider = serviceProvider;
            ChannelCertConfigKit.Initialize();
        }

        private static void ServiceRegister<TService>(IServiceCollection services)
            where TService : class
        {
            ServiceRegister(services, typeof(TService));
        }

        private static void ServiceRegister(IServiceCollection services, Type serviceType)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 获取指定命名空间下的所有类
            Type[] targetTypes = assembly.GetTypes()
                .Where(type => type.BaseType != null && serviceType.IsAssignableFrom(type))
                .ToArray();

            // 注册所有类
            foreach (Type implementationType in targetTypes)
            {
                var instance = Activator.CreateInstance(implementationType);
                var getIfCodeMethod = implementationType.GetMethod("GetIfCode");
                if (getIfCodeMethod != null)
                {
                    var serviceKey = getIfCodeMethod.Invoke(instance, null) as string;
                    if (!string.IsNullOrEmpty(serviceKey))
                    {
                        services.AddKeyedScoped(serviceType, serviceKey, implementationType);
                    }
                }
            }
        }

        private static void ServiceRegister<TService, TAbstractService>(IServiceCollection services, Action<Type> action = null)
            where TService : class
            where TAbstractService : class, TService
        {
            ServiceRegister(services, typeof(TService), typeof(TAbstractService), action);
        }

        private static void ServiceRegister(IServiceCollection services, Type serviceType, Type baseType, Action<Type> action)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 获取指定命名空间下的所有类
            Type[] targetTypes = assembly.GetTypes()
                .Where(type => type.BaseType == baseType && serviceType.IsAssignableFrom(type))
                .ToArray();

            // 注册所有类
            foreach (Type implementationType in targetTypes)
            {
                var instance = Activator.CreateInstance(implementationType);
                var getIfCodeMethod = implementationType.GetMethod("GetIfCode");
                if (getIfCodeMethod != null)
                {
                    var serviceKey = getIfCodeMethod.Invoke(instance, null) as string;
                    if (!string.IsNullOrEmpty(serviceKey))
                    {
                        services.AddKeyedScoped(serviceType, serviceKey, implementationType);
                        action?.Invoke(implementationType);
                    }
                }
            }
        }
    }

    public interface IChannelServiceFactory<T>
    {
        T GetService(object serviceKey);
    }

    public class ChannelServiceFactory<T> : IChannelServiceFactory<T>
    {
        private readonly IServiceProvider _serviceProvider;

        public ChannelServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetService(object serviceKey)
        {
            return _serviceProvider.GetRequiredKeyedService<T>(serviceKey);
        }
    }
}
