
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using AGooday.AgPay.Application.Interfaces;
using Aop.Api.Request;
using Aop.Api.Domain;
using Aop.Api;
using AGooday.AgPay.Payment.Api.Exceptions;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay
{
    /// <summary>
    /// 支付宝 APP支付
    /// </summary>
    public class AliApp : AliPayPaymentService
    {
        public AliApp(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AlipayTradeAppPayRequest req = new AlipayTradeAppPayRequest();
            AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
            model.OutTradeNo = payOrder.PayOrderId;
            model.Subject = payOrder.Subject; //订单标题
            model.Body = payOrder.Body; //订单描述信息
            model.TotalAmount = (Convert.ToDouble(payOrder.Amount) / 100).ToString("0.00");  //支付金额
            req.SetNotifyUrl(GetNotifyUrl()); // 设置异步通知地址
            req.SetBizModel(model);

            //统一放置 isv接口必传信息
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, req, model);


            string payData = null;

            // sdk方式需自行拦截接口异常信息
            try
            {
                payData = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).AlipayClient.SdkExecute(req).Body;
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
