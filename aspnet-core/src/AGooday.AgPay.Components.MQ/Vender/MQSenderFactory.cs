using AGooday.AgPay.Components.MQ.Constant;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AGooday.AgPay.Components.MQ.Vender
{
    public class MQSenderFactory
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
            string mqVender = _configuration[MQVenderCS.MQ_VENDER_KEY];

            switch (mqVender)
            {
                case MQVenderCS.RABBIT_MQ:
                    _configuration.GetSection("MQ:RabbitMQ").Bind(RabbitMQConfig.MQ); 
                    return _serviceProvider.GetRequiredService<RabbitMQSender>();
                default:
                    throw new NotSupportedException("Invalid MQ vender configuration.");
            }
        }
    }
}
