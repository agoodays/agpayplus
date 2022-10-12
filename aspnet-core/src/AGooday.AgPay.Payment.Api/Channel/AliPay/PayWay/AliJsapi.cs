
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Utils;
using Aop.Api.Request;
using Aop.Api.Domain;
using Aop.Api.Response;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Application.Interfaces;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay
{
    public class AliJsapi : AliPayPaymentService
    {
        /// <summary>
        /// 支付宝 条码支付
        /// </summary>
        /// <param name="serviceProvider"></param>
        public AliJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AliJsapiOrderRQ bizRQ = (AliJsapiOrderRQ)rq;

            AlipayTradeCreateRequest req = new AlipayTradeCreateRequest();
            AlipayTradeCreateModel model = new AlipayTradeCreateModel();
            model.OutTradeNo = payOrder.PayOrderId;
            model.Subject = payOrder.Subject; //订单标题
            model.Body = payOrder.Body; //订单描述信息
            model.TotalAmount = (Convert.ToDouble(payOrder.Amount) / 100).ToString("0.00");  //支付金额
            model.BuyerId = bizRQ.BuyerUserId;
            req.SetNotifyUrl(GetNotifyUrl()); // 设置异步通知地址
            req.SetBizModel(model);

            //统一放置 isv接口必传信息
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, req, model);

            //调起支付宝 （如果异常， 将直接跑出   ChannelException ）
            AlipayTradeCreateResponse alipayResp = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(req);

            // 构造函数响应数据
            AliJsapiOrderRS res = ApiResBuilder.BuildSuccess<AliJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            //放置 响应数据
            channelRetMsg.ChannelAttach = alipayResp.Body;

            // ↓↓↓↓↓↓ 调起接口成功后业务判断务必谨慎！！ 避免因代码编写bug，导致不能正确返回订单状态信息  ↓↓↓↓↓↓

            res.AlipayTradeNo = alipayResp.TradeNo;

            channelRetMsg.ChannelOrderId = alipayResp.TradeNo;
            //业务处理成功
            if (!alipayResp.IsError)
            {
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = AliPayKit.AppendErrCode(alipayResp.Code, alipayResp.SubCode);
                channelRetMsg.ChannelErrMsg = AliPayKit.AppendErrMsg(alipayResp.Msg, alipayResp.SubMsg);
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            AliJsapiOrderRQ bizRQ = (AliJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.BuyerUserId))
            {
                throw new BizException("[buyerUserId]不可为空");
            }

            return null;
        }
    }
}
