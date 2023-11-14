using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.AliPay.Kits;
using AGooday.AgPay.Payment.Api.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay
{
    /// <summary>
    /// 支付宝 Pc支付
    /// </summary>
    public class AliPc : AliPayPaymentService
    {
        public AliPc(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AliPcOrderRQ bizRQ = (AliPcOrderRQ)rq;

            AlipayTradePagePayRequest req = new AlipayTradePagePayRequest();
            AlipayTradePagePayModel model = new AlipayTradePagePayModel();
            model.OutTradeNo = payOrder.PayOrderId;
            model.Subject = payOrder.Subject; //订单标题
            model.Body = payOrder.Body; //订单描述信息
            model.TotalAmount = AmountUtil.ConvertCent2Dollar(payOrder.Amount);  //支付金额
            model.ProductCode = "FAST_INSTANT_TRADE_PAY";
            model.QrPayMode = "2"; //订单码-跳转模式
            req.SetNotifyUrl(GetNotifyUrl()); // 设置异步通知地址
            req.SetReturnUrl(GetReturnUrl()); // 同步跳转地址
            req.SetBizModel(model);

            //统一放置 isv接口必传信息
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, req, model);

            // 构造函数响应数据
            AliPcOrderRS res = ApiResBuilder.BuildSuccess<AliPcOrderRS>();

            // sdk方式需自行拦截接口异常信息
            try
            {
                if (CS.PAY_DATA_TYPE.FORM.Equals(bizRQ.PayDataType))
                {
                    res.FormContent = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).AlipayClient.pageExecute(req).Body;
                }
                else
                {
                    res.PayUrl = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).AlipayClient.pageExecute(req, null, "GET").Body;
                }
            }
            catch (AopException e)
            {
                throw ChannelException.SysError(e.Message);
            }

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            //放置 响应数据
            channelRetMsg.ChannelState = ChannelState.WAITING;
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
