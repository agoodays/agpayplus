using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.RQRS.Msg;

namespace AGooday.AgPay.Components.Third.Services
{
    /// <summary>
    /// 订单处理通用逻辑
    /// </summary>
    public class PayOrderProcessService
    {
        private readonly IMQSender _mqSender;
        private readonly IPayOrderService _payOrderService;
        private readonly IAccountBillService _accountBillService;
        private readonly PayMchNotifyService _payMchNotifyService;
        private readonly ILogger<PayOrderProcessService> _logger;

        public PayOrderProcessService(ILogger<PayOrderProcessService> logger,
            IMQSender mqSender,
            IPayOrderService payOrderService,
            IAccountBillService accountBillService,
            PayMchNotifyService payMchNotifyService)
        {
            _mqSender = mqSender;
            _logger = logger;
            _payOrderService = payOrderService;
            _payMchNotifyService = payMchNotifyService;
            _accountBillService = accountBillService;
        }

        public async Task ConfirmSuccessAsync(PayOrderDto payOrder)
        {
            //设置订单状态
            payOrder.State = (byte)PayOrderState.STATE_SUCCESS;

            //记账
            await _accountBillService.GenAccountBillAsync(payOrder.PayOrderId);

            //自动分账 处理逻辑， 不影响主订单任务
            await UpdatePayOrderAutoDivisionAsync(payOrder);

            //发送商户通知
            await _payMchNotifyService.PayOrderNotifyAsync(payOrder);
        }

        /// <summary>
        /// 更新订单自动分账业务
        /// </summary>
        /// <param name="payOrder"></param>
        private async Task UpdatePayOrderAutoDivisionAsync(PayOrderDto payOrder)
        {
            try
            {
                //默认不分账  || 其他非【自动分账】逻辑时， 不处理
                if (payOrder == null || payOrder.DivisionMode == null || payOrder.DivisionMode != (byte)PayOrderDivisionMode.DIVISION_MODE_AUTO)
                {
                    return;
                }

                //更新订单表分账状态为： 等待分账任务处理
                bool updDivisionState = await _payOrderService.UpdateDivisionStateAsync(payOrder);

                if (updDivisionState)
                {
                    //推送到分账MQ
                    await _mqSender.SendAsync(PayOrderDivisionMQ.Build(payOrder.PayOrderId, CS.YES, null), 60); //1分钟后执行
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "订单[{PayOrderId}]自动分账逻辑异常: {Message}", payOrder.PayOrderId, e.Message);
                //_logger.LogError(e, $"订单[{payOrder.PayOrderId}]自动分账逻辑异常: {e.Message}");
            }
        }

        public async Task UpdateIngAndSuccessOrFailByCreatebyOrderAsync(PayOrderDto payOrder, ChannelRetMsg channelRetMsg)
        {
            bool isSuccess = await _payOrderService.UpdateInit2IngAsync(payOrder.PayOrderId, payOrder);
            if (!isSuccess)
            {
                _logger.LogError("updateInit2Ing更新异常 payOrderId={PayOrderId}", payOrder.PayOrderId);
                //_logger.LogError($"updateInit2Ing更新异常 payOrderId={payOrder.PayOrderId}");
                throw new BizException("更新订单异常!");
            }

            isSuccess = await _payOrderService.UpdateIng2SuccessOrFail(payOrder.PayOrderId, payOrder.State,
                    channelRetMsg.ChannelMchNo, channelRetMsg.ChannelIsvNo, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelUserId, channelRetMsg.PlatformOrderId, channelRetMsg.PlatformMchOrderId,
                    channelRetMsg.ChannelErrCode, channelRetMsg.ChannelErrMsg);
            if (!isSuccess)
            {
                _logger.LogError("updateIng2SuccessOrFail更新异常 payOrderId={PayOrderId}", payOrder.PayOrderId);
                //_logger.LogError($"updateIng2SuccessOrFail更新异常 payOrderId={payOrder.PayOrderId}");
                throw new BizException("更新订单异常!");
            }
        }
    }
}
