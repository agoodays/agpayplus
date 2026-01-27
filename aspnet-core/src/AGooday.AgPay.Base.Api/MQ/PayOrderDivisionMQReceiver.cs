using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json;

namespace AGooday.AgPay.Base.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：支付订单分账处理逻辑
    /// </summary>
    public class PayOrderDivisionMQReceiver : PayOrderDivisionMQ.IMQReceiver
    {
        private readonly ILogger<PayOrderDivisionMQReceiver> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PayOrderDivisionMQReceiver(ILogger<PayOrderDivisionMQReceiver> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ReceiveAsync(PayOrderDivisionMQ.MsgPayload payload)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var payOrderDivisionProcessService = scope.ServiceProvider.GetService<PayOrderDivisionProcessService>();
                try
                {
                    _logger.LogInformation("接收订单分账通知MQ, 消息: {payload}", JsonConvert.SerializeObject(payload));
                    //_logger.LogInformation($"接收订单分账通知MQ, 消息: {JsonConvert.SerializeObject(payload)}");
                    await payOrderDivisionProcessService.ProcessPayOrderDivisionAsync(payload.PayOrderId, payload.UseSysAutoDivisionReceivers, payload.ReceiverList, payload.IsResend);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }
    }
}
