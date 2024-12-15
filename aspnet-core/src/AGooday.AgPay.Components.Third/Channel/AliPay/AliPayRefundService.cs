using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 退款接口： 支付宝官方
    /// </summary>
    public class AliPayRefundService : AbstractRefundService
    {
        public AliPayRefundService(ILogger<AliPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public AliPayRefundService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override async Task<ChannelRetMsg> QueryAsync(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            AlipayTradeFastpayRefundQueryRequest request = new AlipayTradeFastpayRefundQueryRequest();
            AlipayTradeFastpayRefundQueryModel model = new AlipayTradeFastpayRefundQueryModel();
            model.TradeNo = refundOrder.ChannelPayOrderNo;
            model.OutTradeNo = refundOrder.PayOrderId;
            model.OutRequestNo = refundOrder.RefundOrderId;
            request.SetBizModel(model);

            //统一放置 isv接口必传信息
            await AliPayKit.PutApiIsvInfoAsync(mchAppConfigContext, request, model);

            AlipayTradeFastpayRefundQueryResponse response = (await _configContextQueryService.GetAlipayClientWrapperAsync(mchAppConfigContext)).Execute(request);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            channelRetMsg.ChannelAttach = response.Body;

            // 调用成功 & 金额相等  （传入不存在的outRequestNo支付宝仍然返回响应成功只是数据不存在， 调用isSuccess() 仍是成功, 此处需判断金额是否相等）
            long? channelRefundAmount = response.RefundAmount == null ? null : Convert.ToInt64(Convert.ToDouble(response.RefundAmount) * 100);
            if (!response.IsError && refundOrder.RefundAmount.Equals(channelRefundAmount))
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.WAITING; //认为是处理中
            }

            return channelRetMsg;
        }

        public override async Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();
            AlipayTradeRefundModel model = new AlipayTradeRefundModel();
            model.OutTradeNo = refundOrder.PayOrderId;
            model.TradeNo = refundOrder.ChannelPayOrderNo;
            model.OutRequestNo = refundOrder.RefundOrderId;
            model.RefundAmount = AmountUtil.ConvertCent2Dollar(refundOrder.RefundAmount);
            model.RefundReason = refundOrder.RefundReason;
            request.SetBizModel(model);

            //统一放置 isv接口必传信息
            await AliPayKit.PutApiIsvInfoAsync(mchAppConfigContext, request, model);

            AlipayTradeRefundResponse response = (await _configContextQueryService.GetAlipayClientWrapperAsync(mchAppConfigContext)).Execute(request);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            channelRetMsg.ChannelAttach = response.Body;

            // 调用成功
            if (!response.IsError)
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = response.SubCode;
                channelRetMsg.ChannelErrMsg = response.SubMsg;
            }
            return channelRetMsg;
        }
    }
}
