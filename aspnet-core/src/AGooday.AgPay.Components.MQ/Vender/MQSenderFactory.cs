﻿using AGooday.AgPay.Components.MQ.Constants;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                    _configuration.GetSection("MQ:RabbitMQ").Bind(RabbitMQConfig.MQ);
                    return _serviceProvider.GetRequiredService<RabbitMQSender>();
                default:
                    throw new NotSupportedException("Invalid MQ vender configuration.");
            }
        }
    }
}
