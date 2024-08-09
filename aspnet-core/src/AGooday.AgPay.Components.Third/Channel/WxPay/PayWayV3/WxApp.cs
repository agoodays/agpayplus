using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;

namespace AGooday.AgPay.Components.Third.Channel.WxPay.PayWayV3
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
            var wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

            // 构造函数响应数据
            WxAppOrderRS res = ApiResBuilder.BuildSuccess<WxAppOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 微信统一下单请求对象
            string subMchAppId = string.Empty, subMerchantId = string.Empty;
            var client = (WechatTenpayClient)wxServiceWrapper.Client;
            CreatePayTransactionAppResponse response;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                var isvSubMchParams = (WxPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                subMchAppId = isvSubMchParams.SubMchAppId;
                subMerchantId = isvSubMchParams.SubMchId;

                var request = new CreatePayPartnerTransactionAppRequest()
                {
                    OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                    AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                    Description = payOrder.Subject,// 订单描述
                    NotifyUrl = GetNotifyUrl(payOrder.PayOrderId),
                    Amount = new CreatePayPartnerTransactionAppRequest.Types.Amount()
                    {
                        Total = Convert.ToInt32(payOrder.Amount),
                        Currency = "CNY"
                    },
                    Scene = new CreatePayPartnerTransactionAppRequest.Types.Scene()
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
                    request.Settlement = new CreatePayPartnerTransactionAppRequest.Types.Settlement()
                    {
                        IsProfitSharing = true,
                    };
                }

                // 子商户subAppId不为空
                if (!string.IsNullOrEmpty(isvSubMchParams.SubMchAppId))
                {
                    request.SubAppId = isvSubMchParams.SubMchAppId;
                }

                // 调起上游接口：
                // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
                // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
                response = client.ExecuteCreatePayPartnerTransactionAppAsync(request).Result;
            }
            else
            {
                var request = new CreatePayTransactionAppRequest()
                {
                    OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                    AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                    Description = payOrder.Subject,// 订单描述
                    ExpireTime = DateTimeOffset.Now.AddMinutes(15),
                    NotifyUrl = GetNotifyUrl(payOrder.PayOrderId),
                    Amount = new CreatePayTransactionAppRequest.Types.Amount()
                    {
                        Total = Convert.ToInt32(payOrder.Amount),
                        Currency = "CNY"
                    },
                };

                //订单分账， 将冻结商户资金。
                if (IsDivisionOrder(payOrder))
                {
                    request.Settlement = new CreatePayPartnerTransactionAppRequest.Types.Settlement()
                    {
                        IsProfitSharing = true,
                    };
                }

                // 调起上游接口：
                // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
                // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
                response = client.ExecuteCreatePayTransactionAppAsync(request).Result;
            }
            if (response.IsSuccessful())
            {
                string wxAppId = wxServiceWrapper.Config.AppId;
                string partnerId = wxServiceWrapper.Config.MchId;
                if (!string.IsNullOrEmpty(subMerchantId))
                {
                    wxAppId = subMchAppId;
                    partnerId = subMerchantId;
                }
                //var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                //var nonceStr = Guid.NewGuid().ToString("N");
                //var payInfo = new Dictionary<string, string>();
                //payInfo.Add("prepayid", response.PrepayId);
                //payInfo.Add("partnerid", partnerId);
                //string packageValue = "Sign=WXPay";
                //payInfo.Add("package", packageValue);
                //payInfo.Add("timeStamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                //payInfo.Add("nonceStr", Guid.NewGuid().ToString("N"));
                //payInfo.Add("appId", wxAppId);
                //string beforeSign = $"{wxAppId}\n{timestamp}\n{nonceStr}\nprepay_id={response.PrepayId}\n";
                //var paySign = WxPayV3Util.RSASign(beforeSign, wxServiceWrapper.Config.MchPrivateKey);
                //payInfo.Add("sign", paySign);// 签名以后在增加prepayId参数
                //payInfo.Add("prepayId", response.PrepayId);
                var payInfo = client.GenerateParametersForAppPayRequest(partnerId,wxAppId, response.PrepayId);
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
            return null;
        }
    }
}
