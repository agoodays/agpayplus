using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：支付订单分账处理逻辑
    /// </summary>
    public class PayOrderDivisionMQReceiver : PayOrderDivisionMQ.IMQReceiver
    {
        private readonly ILogger<PayOrderDivisionMQReceiver> log;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PayOrderDivisionMQReceiver(ILogger<PayOrderDivisionMQReceiver> log,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.log = log;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Receive(PayOrderDivisionMQ.MsgPayload payload)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var payOrderDivisionProcessService = scope.ServiceProvider.GetService<PayOrderDivisionProcessService>();
                try
                {
                    log.LogInformation($"接收订单分账通知MQ, msg={JsonConvert.SerializeObject(payload)}");
                    payOrderDivisionProcessService.ProcessPayOrderDivision(payload.PayOrderId, payload.UseSysAutoDivisionReceivers, payload.ReceiverList, payload.IsResend);
                }
                catch (Exception e)
                {
                    log.LogError(e, e.Message);
                }
            }
        }
    }
}
