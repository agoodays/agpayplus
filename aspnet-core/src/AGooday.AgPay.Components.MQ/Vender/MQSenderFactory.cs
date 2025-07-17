using AGooday.AgPay.Components.MQ.Constants;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AGooday.AgPay.Components.MQ.Vender
{
    public interface IMQSenderFactory
    {
        IMQSender CreateSender();
    }

    public class MQSenderFactory : IMQSenderFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public MQSenderFactory(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public IMQSender CreateSender()
        {
            string mqVender = _configuration[MQVenderCS.MQ_VENDER_KEY] ?? throw new InvalidOperationException("MQ vender configuration is missing.");

            switch (mqVender)
            {
                case MQVenderCS.RABBIT_MQ:
                    // 创建配置对象并绑定
                    var rabbitMQConfig = new RabbitMQConfig();
                    _configuration.GetSection("MQ:RabbitMQ").Bind(rabbitMQConfig.MQ);

                    // 使用服务提供者创建 RabbitMQSender 实例
                    return ActivatorUtilities.CreateInstance<RabbitMQSender>(
                        _serviceProvider,
                        Options.Create(rabbitMQConfig));
                default:
                    throw new NotSupportedException("Invalid MQ vender configuration.");
            }
        }
    }
}
