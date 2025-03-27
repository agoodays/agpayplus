using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：支付订单补单（一般用于没有回调的接口，比如微信的条码支付）
    /// </summary>
    public class PayOrderReissueMQReceiver : PayOrderReissueMQ.IMQReceiver
    {
        private readonly IMQSender _mqSender;
        private readonly ILogger<PayOrderReissueMQReceiver> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PayOrderReissueMQReceiver(ILogger<PayOrderReissueMQReceiver> logger,
            IMQSender mqSender,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _mqSender = mqSender;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ReceiveAsync(PayOrderReissueMQ.MsgPayload payload)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var payOrderService = scope.ServiceProvider.GetService<IPayOrderService>();
                var channelOrderReissueService = scope.ServiceProvider.GetService<ChannelOrderReissueService>();
                try
                {
                    _logger.LogInformation("接收轮询查单通知MQ, 消息: {payload}", JsonConvert.SerializeObject(payload));
                    //_logger.LogInformation($"接收轮询查单通知MQ, 消息: {JsonConvert.SerializeObject(payload)}");
                    string payOrderId = payload.PayOrderId;
                    int currentCount = payload.Count;
                    currentCount++;

                    PayOrderDto payOrder = await payOrderService.GetByIdAsync(payOrderId);
                    if (payOrder == null)
                    {
                        _logger.LogWarning("查询支付订单为空 payOrderId={payOrderId}", payOrderId);
                        //_logger.LogWarning($"查询支付订单为空 payOrderId={payOrderId}");
                        return;
                    }

                    if (payOrder.State != (byte)PayOrderState.STATE_ING)
                    {
                        _logger.LogWarning("订单状态不是支付中, 不需查询渠道 payOrderId={payOrderId}", payOrderId);
                        //_logger.LogWarning($"订单状态不是支付中, 不需查询渠道 payOrderId={payOrderId}");
                        return;
                    }

                    ChannelRetMsg channelRetMsg = await channelOrderReissueService.ProcessPayOrderAsync(payOrder);

                    //返回null 可能为接口报错等， 需要再次轮询
                    if (channelRetMsg == null || channelRetMsg.ChannelState == null || channelRetMsg.ChannelState.Equals(ChannelState.WAITING))
                    {
                        //最多查询6次
                        if (currentCount <= 6)
                        {
                            await _mqSender.SendAsync(PayOrderReissueMQ.Build(payOrderId, currentCount), 5); //延迟5s再次查询
                        }
                        else
                        {
                            //TODO 调用【撤销订单】接口
                        }
                    }
                    else
                    {
                        //其他状态， 不需要再次轮询。
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    return;
                }
            }
        }
    }
}
