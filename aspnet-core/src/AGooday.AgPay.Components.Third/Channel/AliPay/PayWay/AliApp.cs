﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;

namespace AGooday.AgPay.Components.Third.Channel.AliPay.PayWay
{
    /// <summary>
    /// 支付宝 APP支付
    /// </summary>
    public class AliApp : AliPayPaymentService
    {
        public AliApp(ILogger<AliApp> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AlipayTradeAppPayRequest req = new AlipayTradeAppPayRequest();
            AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
            model.OutTradeNo = payOrder.PayOrderId;
            model.Subject = payOrder.Subject; //订单标题
            model.Body = payOrder.Body; //订单描述信息
            model.TotalAmount = AmountUtil.ConvertCent2Dollar(payOrder.Amount);  //支付金额
            req.SetNotifyUrl(GetNotifyUrl()); // 设置异步通知地址
            req.SetBizModel(model);

            //统一放置 isv接口必传信息
            await AliPayKit.PutApiIsvInfoAsync(mchAppConfigContext, req, model);


            string payData;

            // sdk方式需自行拦截接口异常信息
            try
            {
                payData = (await _configContextQueryService.GetAlipayClientWrapperAsync(mchAppConfigContext)).AlipayClient.SdkExecute(req).Body;
            }
            catch (AopException e)
            {
                throw ChannelException.SysError(e.Message);
            }

            // 构造函数响应数据
            AliAppOrderRS res = ApiResBuilder.BuildSuccess<AliAppOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            //放置 响应数据
            channelRetMsg.ChannelAttach = payData;
            channelRetMsg.ChannelState = ChannelState.WAITING;
            res.PayData = payData;
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
