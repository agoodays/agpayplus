using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Utils;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.Services;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3
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

            var wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

            // 构造函数响应数据
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 微信统一下单请求对象
            CreatePayTransactionJsapiResponse response;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                var isvSubMchParams = (WxPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                var request = new CreatePayPartnerTransactionJsapiRequest()
                {
                    OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                    AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                    Description = payOrder.Subject,// 订单描述
                    NotifyUrl = GetNotifyUrl(payOrder.PayOrderId),
                    Amount = new CreatePayPartnerTransactionJsapiRequest.Types.Amount()
                    {
                        Total = Convert.ToInt32(payOrder.Amount),
                        Currency = "CNY"
                    },
                    Scene = new CreatePayPartnerTransactionJsapiRequest.Types.Scene()
                    {
                        ClientIp = payOrder.ClientIp,
                    },
                    // 添加子商户参数
                    SubMerchantId = isvSubMchParams.SubMchId,
                    ExpireTime = DateTimeOffset.Now.AddMinutes(15),
                };

                //订单分账， 将冻结商户资金。
                if (IsDivisionOrder(payOrder))
                {
                    request.Settlement = new CreatePayPartnerTransactionJsapiRequest.Types.Settlement()
                    {
                        IsProfitSharing = true,
                    };
                }

                var payer = new CreatePayPartnerTransactionJsapiRequest.Types.Payer()
                {
                    SubOpenId = bizRQ.Openid,
                    OpenId = bizRQ.Openid,
                };
                // 子商户subAppId不为空
                if (!string.IsNullOrEmpty(isvSubMchParams.SubMchAppId))
                {
                    request.SubAppId = isvSubMchParams.SubMchAppId;
                    payer.SubOpenId = bizRQ.Openid;// 用户在子商户appid下的唯一标识
                }
                else
                {
                    payer.OpenId = bizRQ.Openid;// 用户在服务商appid下的唯一标识
                }
                request.Payer = payer;

                // 调起上游接口：
                // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
                // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
                response = ((WechatTenpayClient)wxServiceWrapper.Client).ExecuteCreatePayPartnerTransactionJsapiAsync(request).Result;
            }
            else
            {
                var request = new CreatePayTransactionJsapiRequest()
                {
                    OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                    AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                    Description = payOrder.Subject,// 订单描述
                    ExpireTime = DateTimeOffset.Now.AddMinutes(15),
                    NotifyUrl = GetNotifyUrl(payOrder.PayOrderId),
                    Amount = new CreatePayTransactionJsapiRequest.Types.Amount()
                    {
                        Total = Convert.ToInt32(payOrder.Amount),
                        Currency = "CNY"
                    },
                    Payer = new CreatePayTransactionJsapiRequest.Types.Payer()
                    {
                        OpenId = bizRQ.Openid
                    }
                };

                //订单分账， 将冻结商户资金。
                if (IsDivisionOrder(payOrder))
                {
                    request.Settlement = new CreatePayPartnerTransactionJsapiRequest.Types.Settlement()
                    {
                        IsProfitSharing = true,
                    };
                }

                // 调起上游接口：
                // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
                // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
                response = ((WechatTenpayClient)wxServiceWrapper.Client).ExecuteCreatePayTransactionJsapiAsync(request).Result;
            }
            if (response.IsSuccessful())
            {
                var appid = wxServiceWrapper.Config.AppId;
                var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                var nonceStr = Guid.NewGuid().ToString("N");
                var payInfo = new Dictionary<string, string>();
                payInfo.Add("appId", appid);
                payInfo.Add("timeStamp", timestamp);
                payInfo.Add("nonceStr", nonceStr);
                payInfo.Add("package", $"prepay_id={response.PrepayId}");
                payInfo.Add("signType", "RSA");
                string beforeSign = $"{appid}\n{timestamp}\n{nonceStr}\nprepay_id={response.PrepayId}\n";
                var paySign = WxPayV3Util.RSASign(beforeSign, wxServiceWrapper.Config.MchPrivateKey);
                payInfo.Add("paySign", paySign);// 签名以后在增加prepayId参数
                payInfo.Add("prepayId", response.PrepayId);
                res.PayInfo = JsonConvert.SerializeObject(payInfo);
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = response.ErrorCode;
                channelRetMsg.ChannelErrMsg = response.ErrorMessage;
            }

            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.Openid))
            {
                throw new BizException("[openid]不可为空");
            }

            return null;
        }
    }
}
