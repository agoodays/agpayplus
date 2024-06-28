using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay
{
    /// <summary>
    /// 微信 native支付
    /// </summary>
    public class WxNative : WxPayPaymentService
    {
        public WxNative(ILogger<WxNative> logger, 
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxNativeOrderRQ bizRQ = (WxNativeOrderRQ)rq;

            var request = BuildUnifiedOrderRequest(payOrder, mchAppConfigContext, out WxServiceWrapper wxServiceWrapper);
            request.TradeType = "NATIVE";

            // 构造函数响应数据
            WxNativeOrderRS res = ApiResBuilder.BuildSuccess<WxNativeOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 调起上游接口：
            // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
            // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
            var response = ((WechatTenpayClient)wxServiceWrapper.Client).ExecuteCreatePayUnifiedOrderAsync(request).Result;
            if (response.IsSuccessful())
            {
                string payUrl = response.MobileWebUrl;
                if (CS.PAY_DATA_TYPE.FORM.Equals(bizRQ.PayDataType))
                {
                    //表单方式
                    res.FormContent = payUrl;
                }
                else if (CS.PAY_DATA_TYPE.CODE_IMG_URL.Equals(bizRQ.PayDataType))
                {
                    //二维码图片地址
                    res.CodeImgUrl = _sysConfigService.GetDBApplicationConfig().GenScanImgUrl(payUrl);
                }
                else
                {
                    // 默认都为 payUrl方式
                    res.PayUrl = payUrl;
                }
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = WxPayKit.AppendErrCode(response.ReturnCode, response.ErrorCode); //优先： subCode
                var msg = "OK".Equals(response.ReturnMessage, StringComparison.CurrentCultureIgnoreCase) ? null : response.ReturnMessage;
                var subMsg = response.ErrorCodeDescription;
                channelRetMsg.ChannelErrMsg = WxPayKit.AppendErrMsg(subMsg, msg);
            }

            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
