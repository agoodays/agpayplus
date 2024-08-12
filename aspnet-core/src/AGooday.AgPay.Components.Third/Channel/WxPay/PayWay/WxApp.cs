using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.Third.Channel.WxPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;

namespace AGooday.AgPay.Components.Third.Channel.WxPay.PayWay
{
    /// <summary>
    /// 微信 app支付
    /// </summary>
    public class WxApp : WxPayPaymentService
    {
        public WxApp(ILogger<WxApp> logger, 
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            var request = BuildUnifiedOrderRequest(payOrder, mchAppConfigContext, out WxServiceWrapper wxServiceWrapper);
            request.TradeType = "APP";

            // 构造函数响应数据
            WxAppOrderRS res = ApiResBuilder.BuildSuccess<WxAppOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 调起上游接口：
            // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
            // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
            var client = (WechatTenpayClient)wxServiceWrapper.Client;
            var response = client.ExecuteCreatePayUnifiedOrderAsync(request).Result;
            if (response.IsSuccessful())
            {
                //var payInfo = new Dictionary<string, string>();

                //// 此map用于参与调起sdk支付的二次签名,格式全小写，timestamp只能是10位,格式固定，切勿修改
                string partnerId = response.MerchantId;
                if (!string.IsNullOrEmpty(response.SubMerchantId))
                {
                    partnerId = response.SubMerchantId;
                }
                //payInfo.Add("prepayid", response.PrepayId);
                //payInfo.Add("partnerid", partnerId);
                //string packageValue = "Sign=WXPay";
                //payInfo.Add("package", packageValue);
                //payInfo.Add("timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                //payInfo.Add("noncestr", Guid.NewGuid().ToString("N"));
                //payInfo.Add("appid", response.AppId);
                //var paySign = WxPayKit.Sign(payInfo, wxServiceWrapper.Config.MchKey);
                //payInfo.Add("sign", paySign);
                var payInfo = client.GenerateParametersForAppGetBrandPayRequest(partnerId,request.AppId, response.PrepayId, request.SignType);
                res.PayInfo = JsonConvert.SerializeObject(payInfo);
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
