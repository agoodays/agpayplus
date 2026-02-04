using AGooday.AgPay.Base.Api.MQ;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;

namespace AGooday.AgPay.Base.Api.Extensions
{
    /// <summary>
    /// RabbitMQ 配置扩展方法
    /// </summary>
    public static class RabbitMQExtensions
    {
        /// <summary>
        /// 添加 RabbitMQ 基础服务（Sender + Receiver 基础设施）
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="rabbitMQReceiverTypes">RabbitMQ 通用 Receiver 类型数组（实现 IMQMsgReceiver）</param>
        /// <param name="specificReceiverTypes">特定接口的 Receiver 类型元组数组</param>
        public static IServiceCollection AddRabbitMQServices(
            this IServiceCollection services,
            Type[] rabbitMQReceiverTypes = null,
            (Type serviceType, Type implementationType)[] specificReceiverTypes = null)
        {
            // 注册 Sender
            services.AddTransient<RabbitMQSender>();
            services.AddSingleton<IMQSenderFactory, MQSenderFactory>();
            services.AddSingleton<IMQSender>(provider =>
            {
                var factory = provider.GetRequiredService<IMQSenderFactory>();
                return factory.CreateSender();
            });

            // 动态注册通用 Receiver（实现 IMQMsgReceiver 接口）
            if (rabbitMQReceiverTypes != null && rabbitMQReceiverTypes.Length > 0)
            {
                foreach (var type in rabbitMQReceiverTypes)
                {
                    services.AddSingleton(typeof(IMQMsgReceiver), type);
                }
            }

            // 动态注册特定接口的 Receiver
            if (specificReceiverTypes != null && specificReceiverTypes.Length > 0)
            {
                foreach (var (serviceType, implementationType) in specificReceiverTypes)
                {
                    services.AddSingleton(serviceType, implementationType);
                }
            }

            // 注册 HostedService（监听和处理消息）
            services.AddHostedService<MQReceiverHostedService>();

            return services;
        }
    }
}
