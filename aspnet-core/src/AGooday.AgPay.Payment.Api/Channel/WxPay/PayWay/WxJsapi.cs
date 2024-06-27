using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay
{
    /// <summary>
    /// 微信 jsapi支付
    /// </summary>
    public class WxJsapi : WxPayPaymentService
    {
        public WxJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;

            var request = BuildUnifiedOrderRequest(payOrder, mchAppConfigContext, out WxServiceWrapper wxServiceWrapper);
            request.TradeType = "JSAPI";
            if (mchAppConfigContext.IsIsvSubMch() && !String.IsNullOrWhiteSpace(request.SubAppId))// 特约商户 && 传了子商户appId
            {
                request.SubOpenId = bizRQ.GetChannelUserId();// 用户在子商户appid下的唯一标识
            }
            else
            {
                request.OpenId = bizRQ.GetChannelUserId();
            }

            // 构造函数响应数据
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();
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
                //payInfo.Add("appId", response.AppId);
                //payInfo.Add("timeStamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                //payInfo.Add("nonceStr", Guid.NewGuid().ToString("N"));
                //payInfo.Add("package", $"prepay_id={response.PrepayId}");
                //payInfo.Add("signType", "MD5");
                //var paySign = WxPayKit.Sign(payInfo, wxServiceWrapper.Config.MchKey);
                //payInfo.Add("paySign", paySign);
                // 下面的参数字典可直接以 JSON 格式返回给客户端，客户端反序列化后再原样传递给 wx.chooseWXPay() 方法即可
                var payInfo = client.GenerateParametersForJsapiGetBrandPayRequest(request.AppId, response.PrepayId, request.SignType);
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
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[openid]不可为空");
            }

            return null;
        }
    }
}
