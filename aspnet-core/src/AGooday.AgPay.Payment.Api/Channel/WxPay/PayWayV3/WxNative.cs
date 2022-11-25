using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3
{
    /// <summary>
    /// 微信 native支付
    /// </summary>
    public class WxNative : WxPayPaymentService
    {
        public WxNative(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxNativeOrderRQ bizRQ = (WxNativeOrderRQ)rq;

            var wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

            // 构造函数响应数据
            WxNativeOrderRS res = ApiResBuilder.BuildSuccess<WxNativeOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 微信统一下单请求对象
            CreatePayTransactionNativeResponse response;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                var isvSubMchParams = (WxPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                var request = new CreatePayPartnerTransactionNativeRequest()
                {
                    OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                    AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                    Description = payOrder.Subject,// 订单描述
                    NotifyUrl = GetNotifyUrl(payOrder.PayOrderId),
                    Amount = new CreatePayPartnerTransactionNativeRequest.Types.Amount()
                    {
                        Total = Convert.ToInt32(payOrder.Amount),
                        Currency = "CNY"
                    },
                    Scene = new CreatePayPartnerTransactionNativeRequest.Types.Scene()
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
                    request.Settlement = new CreatePayPartnerTransactionNativeRequest.Types.Settlement()
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
                response = ((WechatTenpayClient)wxServiceWrapper.Client).ExecuteCreatePayPartnerTransactionNativeAsync(request).Result;
            }
            else
            {
                var request = new CreatePayTransactionNativeRequest()
                {
                    OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                    AppId = wxServiceWrapper.Config.AppId,// 微信 AppId
                    Description = payOrder.Subject,// 订单描述
                    ExpireTime = DateTimeOffset.Now.AddMinutes(15),
                    NotifyUrl = GetNotifyUrl(payOrder.PayOrderId),
                    Amount = new CreatePayTransactionNativeRequest.Types.Amount()
                    {
                        Total = Convert.ToInt32(payOrder.Amount),
                        Currency = "CNY"
                    },
                    Scene = new CreatePayPartnerTransactionNativeRequest.Types.Scene()
                    {
                        ClientIp = payOrder.ClientIp,
                    },
                };

                //订单分账， 将冻结商户资金。
                if (IsDivisionOrder(payOrder))
                {
                    request.Settlement = new CreatePayPartnerTransactionNativeRequest.Types.Settlement()
                    {
                        IsProfitSharing = true,
                    };
                }

                // 调起上游接口：
                // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
                // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
                response = ((WechatTenpayClient)wxServiceWrapper.Client).ExecuteCreatePayTransactionNativeAsync(request).Result;
            }
            if (response.IsSuccessful())
            {
                string codeUrl = response.QrcodeUrl;
                if (CS.PAY_DATA_TYPE.CODE_IMG_URL.Equals(bizRQ.PayDataType))
                {
                    // 二维码图片地址
                    res.CodeImgUrl = _sysConfigService.GetDBApplicationConfig().GenScanImgUrl(codeUrl);
                }
                else
                {
                    // 默认都为 codeUrl方式
                    res.CodeUrl = codeUrl;
                }

                // 支付中
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
