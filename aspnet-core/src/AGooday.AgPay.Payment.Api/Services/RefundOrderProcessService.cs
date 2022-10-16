using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using System;

namespace AGooday.AgPay.Payment.Api.Services
{
    /// <summary>
    /// 退款处理通用逻辑
    /// </summary>
    public class RefundOrderProcessService
    {
        private readonly PayMchNotifyService payMchNotifyService;
        private readonly IRefundOrderService refundOrderService;

        public RefundOrderProcessService(PayMchNotifyService payMchNotifyService, IRefundOrderService refundOrderService)
        {
            this.refundOrderService = refundOrderService;
            this.payMchNotifyService = payMchNotifyService;
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
                updateOrderSuccess = refundOrderService.UpdateIng2Success(refundOrderId, channelRetMsg.ChannelOrderId);
                if (updateOrderSuccess)
                {
                    // 通知商户系统
                    if (!string.IsNullOrWhiteSpace(refundOrder.NotifyUrl))
                    {
                        payMchNotifyService.RefundOrderNotify(refundOrderService.GetById(refundOrderId));
                    }
                }
            }
            //确认失败
            else if (channelRetMsg.ChannelState == ChannelState.CONFIRM_FAIL)
            {
                // 更新为失败状态
                updateOrderSuccess = refundOrderService.UpdateIng2Fail(refundOrderId, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelErrCode, channelRetMsg.ChannelErrMsg);
                // 通知商户系统
                if (!string.IsNullOrWhiteSpace(refundOrder.NotifyUrl))
                {
                    payMchNotifyService.RefundOrderNotify(refundOrderService.GetById(refundOrderId));
                }
            }
            return updateOrderSuccess;
        }
    }
}
