using AGooday.AgPay.Components.MQ.Models;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.MQ
{
    public class PayOrderDivisionMQReceiver : PayOrderDivisionMQ.IMQReceiver
    {
        private readonly ILogger<PayOrderDivisionMQReceiver> log;

        public PayOrderDivisionMQReceiver(ILogger<PayOrderDivisionMQReceiver> log)
        {
            this.log = log;
        }

        public void Receive(PayOrderDivisionMQ.MsgPayload payload)
        {
            log.LogInformation($"接收商户通知MQ, msg={JsonConvert.SerializeObject(payload)}");
        }
    }
}
