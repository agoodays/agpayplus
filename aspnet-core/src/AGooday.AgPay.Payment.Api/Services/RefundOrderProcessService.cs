using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Services
{
    /// <summary>
    /// 退款处理通用逻辑
    /// </summary>
    public class RefundOrderProcessService
    {
        private readonly PayMchNotifyService payMchNotifyService;
        private readonly IPayOrderService _payOrderService;
        private readonly IRefundOrderService _refundOrderService;
        private readonly IPayOrderProfitService _payOrderProfitService;
        private readonly IAccountBillService _accountBillService;
        private readonly Func<string, IRefundService> _refundServiceFactory;

        public RefundOrderProcessService(PayMchNotifyService payMchNotifyService,
            IPayOrderService payOrderService,
            IRefundOrderService refundOrderService,
            IPayOrderProfitService payOrderProfitService,
            Func<string, IRefundService> refundServiceFactory,
            IAccountBillService accountBillService)
        {
            this.payMchNotifyService = payMchNotifyService;
            _payOrderService = payOrderService;
            _refundOrderService = refundOrderService;
            _payOrderProfitService = payOrderProfitService;
            _refundServiceFactory = refundServiceFactory;
            _accountBillService = accountBillService;
        }

        /// <summary>
        /// 根据通道返回的状态，处理退款订单业务
        /// </summary>
        /// <param name="channelRetMsg"></param>
        /// <param name="refundOrder"></param>
        public bool HandleRefundOrder4Channel(ChannelRetMsg channelRetMsg, RefundOrderDto refundOrder)
        {
            bool updateOrderSuccess = true; //默认更新成功
            string refundOrderId = refundOrder.RefundOrderId;
            // 明确退款成功
            if (channelRetMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
            {
                updateOrderSuccess = _refundOrderService.UpdateIng2Success(refundOrderId, channelRetMsg.ChannelOrderId);
                if (updateOrderSuccess)
                {
                    this.UpdatePayOrderProfit(refundOrder);
                    // 通知商户系统
                    if (!string.IsNullOrWhiteSpace(refundOrder.NotifyUrl))
                    {
                        payMchNotifyService.RefundOrderNotify(_refundOrderService.GetById(refundOrderId));
                    }
                }
            }
            //确认失败
            else if (channelRetMsg.ChannelState == ChannelState.CONFIRM_FAIL)
            {
                // 更新为失败状态
                updateOrderSuccess = _refundOrderService.UpdateIng2Fail(refundOrderId, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelErrCode, channelRetMsg.ChannelErrMsg);
                // 通知商户系统
                if (!string.IsNullOrWhiteSpace(refundOrder.NotifyUrl))
                {
                    payMchNotifyService.RefundOrderNotify(_refundOrderService.GetById(refundOrderId));
                }
            }
            return updateOrderSuccess;
        }

        /// <summary>
        /// 更新支付订单分润
        /// </summary>
        /// <param name="refundOrder"></param>
        public void UpdatePayOrderProfit(RefundOrderDto refundOrder)
        {
            IRefundService refundService = _refundServiceFactory(refundOrder.IfCode);
            var payOrder = _payOrderService.GetById(refundOrder.PayOrderId);
            var payOrderProfits = _payOrderProfitService.GetByPayOrderId(refundOrder.PayOrderId);
            var amount = payOrder.Amount - payOrder.RefundAmount;
            foreach (var payOrderProfit in payOrderProfits)
            {
                var beforeBalance = payOrderProfit.ProfitAmount;
                var profitAmount = refundService?.CalculateProfitAmount(amount, payOrderProfit.ProfitRate) ?? 0;
                var afterBalance = profitAmount;
                var changeAmount = afterBalance - beforeBalance;
                payOrderProfit.ProfitAmount = profitAmount;
                _payOrderProfitService.Update(payOrderProfit);

                if (payOrderProfit.ProfitAmount > 0)
                {
                    var accountBill = new AccountBillDto();
                    accountBill.InfoId = payOrderProfit.InfoId;
                    accountBill.InfoName = payOrderProfit.InfoName;
                    accountBill.InfoType = payOrderProfit.InfoType;
                    accountBill.BeforeBalance = beforeBalance;
                    accountBill.ChangeAmount = changeAmount;
                    accountBill.AfterBalance = afterBalance;
                    accountBill.BizType = (byte)AccountBillBizType.REFUND_OFFSET;
                    accountBill.AccountType = (byte)AccountBillAccountType.IN_TRANSIT_ACCOUNT;
                    accountBill.RelaBizOrderType = (byte)AccountBillRelaBizOrderType.REFUND_ORDER;
                    accountBill.RelaBizOrderId = refundOrder.RefundOrderId;
                    _accountBillService.Add(accountBill);
                }
            }
        }
    }
}
