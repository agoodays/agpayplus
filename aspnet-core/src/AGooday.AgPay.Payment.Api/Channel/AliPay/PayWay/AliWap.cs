
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
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay
{
    /// <summary>
    /// 支付宝 APP支付
    /// </summary>
    public class AliWap : AliPayPaymentService
    {
        public AliWap(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AliWapOrderRQ bizRQ = (AliWapOrderRQ)rq;

            AlipayTradeWapPayRequest req = new AlipayTradeWapPayRequest();
            AlipayTradeWapPayModel model = new AlipayTradeWapPayModel();
            model.OutTradeNo = payOrder.PayOrderId;
            model.Subject = payOrder.Subject; //订单标题
            model.Body = payOrder.Body; //订单描述信息
            model.TotalAmount = AmountUtil.ConvertCent2Dollar(payOrder.Amount);  //支付金额
            model.ProductCode = "QUICK_WAP_PAY";
            req.SetNotifyUrl(GetNotifyUrl()); // 设置异步通知地址
            req.SetReturnUrl(GetReturnUrl()); // 同步跳转地址
            req.SetBizModel(model);

            //统一放置 isv接口必传信息
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, req, model);

            // 构造函数响应数据
            AliWapOrderRS res = ApiResBuilder.BuildSuccess<AliWapOrderRS>();

            try
            {
                //表单方式
                if (CS.PAY_DATA_TYPE.FORM.Equals(bizRQ.PayDataType))
                {
                    res.FormContent = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).AlipayClient.pageExecute(req).Body;

                }
                //二维码图片地址
                else if (CS.PAY_DATA_TYPE.CODE_IMG_URL.Equals(bizRQ.PayDataType))
                {

                    string payUrl = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).AlipayClient.pageExecute(req, null, "GET").Body;
                    res.CodeImgUrl = _sysConfigService.GetDBApplicationConfig().GenScanImgUrl(payUrl);
                }
                else
                { // 默认都为 payUrl方式

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
