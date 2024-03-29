using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
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
        private readonly IChannelServiceFactory<IPaymentService> _paymentServiceFactory;

        public RefundOrderProcessService(PayMchNotifyService payMchNotifyService,
            IPayOrderService payOrderService,
            IRefundOrderService refundOrderService,
            IPayOrderProfitService payOrderProfitService,
            IChannelServiceFactory<IPaymentService> paymentServiceFactory,
            IAccountBillService accountBillService)
        {
            this.payMchNotifyService = payMchNotifyService;
            _payOrderService = payOrderService;
            _refundOrderService = refundOrderService;
            _payOrderProfitService = payOrderProfitService;
            _paymentServiceFactory = paymentServiceFactory;
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
                    this.UpdatePayOrderProfitAndGenAccountBill(refundOrder);
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
        /// 更新支付订单分润并生成账单
        /// </summary>
        /// <param name="refundOrder"></param>
        public void UpdatePayOrderProfitAndGenAccountBill(RefundOrderDto refundOrder)
        {
            IPaymentService paymentService = _paymentServiceFactory.GetService(refundOrder.IfCode);
            var payOrder = _payOrderService.GetById(refundOrder.PayOrderId);
            var payOrderProfits = _payOrderProfitService.GetByPayOrderIdAsNoTracking(refundOrder.PayOrderId);
            var amount = payOrder.Amount - payOrder.RefundAmount;
            var beforeBalance = 0L;
            var profitAmount = 0L;
            var afterBalance = 0L;
            var changeAmount = 0L;
            var totalProfitAmount = 0L;
            var agentPayOrderProfits = payOrderProfits.Where(w => w.InfoType.Equals(CS.PAY_ORDER_PROFIT_INFO_TYPE.AGENT)).OrderBy(o => o.Id);
            foreach (var payOrderProfit in agentPayOrderProfits)
            {
                beforeBalance = payOrderProfit.ProfitAmount;
                profitAmount = paymentService?.CalculateProfitAmount(amount, payOrderProfit.ProfitRate) ?? 0;
                afterBalance = profitAmount;
                changeAmount = afterBalance - beforeBalance;
                payOrderProfit.FeeAmount = paymentService?.CalculateFeeAmount(amount, payOrderProfit.FeeRate) ?? 0;
                payOrderProfit.ProfitAmount = profitAmount;
                _payOrderProfitService.Update(payOrderProfit);
                this.GenAccountBill(payOrderProfit, refundOrder, beforeBalance, afterBalance, changeAmount);
                totalProfitAmount += profitAmount;
            }

            var platformInaccountPayOrderProfit = payOrderProfits
                .Where(w => w.InfoType.Equals(CS.PAY_ORDER_PROFIT_INFO_TYPE.PLATFORM) && w.InfoId.Equals(CS.PAY_ORDER_PROFIT_INFO_ID.PLATFORM_INACCOUNT))
                .OrderBy(o => o.Id).FirstOrDefault();
            var isvFeeAmount = paymentService?.CalculateFeeAmount(amount, platformInaccountPayOrderProfit.FeeRate) ?? 0;
            var platformInaccountProfitAmount = payOrder.MchFeeAmount - isvFeeAmount;
            beforeBalance = platformInaccountPayOrderProfit.ProfitAmount;
            profitAmount = platformInaccountProfitAmount;
            afterBalance = profitAmount;
            changeAmount = afterBalance - beforeBalance;
            platformInaccountPayOrderProfit.FeeAmount = paymentService?.CalculateFeeAmount(amount, platformInaccountPayOrderProfit.FeeRate) ?? 0;
            platformInaccountPayOrderProfit.ProfitAmount = profitAmount;
            _payOrderProfitService.Update(platformInaccountPayOrderProfit);
            this.GenAccountBill(platformInaccountPayOrderProfit, refundOrder, beforeBalance, afterBalance, changeAmount);

            var platformProfitPayOrderProfit = payOrderProfits
                .Where(w => w.InfoType.Equals(CS.PAY_ORDER_PROFIT_INFO_TYPE.PLATFORM) && w.InfoId.Equals(CS.PAY_ORDER_PROFIT_INFO_ID.PLATFORM_PROFIT))
                .OrderBy(o => o.Id).FirstOrDefault();
            var platformProfitAmount = platformInaccountProfitAmount - totalProfitAmount;
            beforeBalance = platformProfitPayOrderProfit.ProfitAmount;
            profitAmount = platformProfitAmount;
            afterBalance = profitAmount;
            changeAmount = afterBalance - beforeBalance;
            platformProfitPayOrderProfit.FeeAmount = paymentService?.CalculateFeeAmount(amount, platformProfitPayOrderProfit.FeeRate) ?? 0;
            platformProfitPayOrderProfit.ProfitAmount = profitAmount;
            _payOrderProfitService.Update(platformProfitPayOrderProfit);
            this.GenAccountBill(platformProfitPayOrderProfit, refundOrder, beforeBalance, afterBalance, changeAmount);
        }

        private void GenAccountBill(PayOrderProfitDto payOrderProfit, RefundOrderDto refundOrder, long beforeBalance, long afterBalance, long changeAmount)
        {
            if (beforeBalance > 0 && changeAmount != 0)
            {
                var accountBill = new AccountBillDto();
                accountBill.BillId = SeqUtil.GenBillId();
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
