using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.MQ
{
    public class PayOrderReissueMQReceiver
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<PayOrderMchNotifyMQReceiver> log;
        private readonly ChannelOrderReissueService channelOrderReissueService;
    }
}
