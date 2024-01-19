using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.UmsPay;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay.PayWay
{
    /// <summary>
    /// 银联商务 微信 jsapi
    /// </summary>
    public class WxJsapi : UmsPayPaymentService
    {
        public WxJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【银联商务(wechatJs)jsapi支付】";
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            // 构造函数响应数据
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();

            JObject reqParams = new JObject();
            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            //用户子标识 微信必传，需要商户自行调用微信平台接口获取，具体获取方式 请根据微信接口文档
            reqParams.Add("subOpenId", bizRQ.Openid);//用户ID

            // 获取微信官方配置的 appId
            UmsPayIsvSubMchParams umspayIsvParams = (UmsPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            // 微信子商户appId
            reqParams.Add("subAppId", umspayIsvParams.SubMchAppId);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/v1/netpay/wx/unified-order", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            resJSON.TryGetString("errInfo", out string errInfo); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("seqId", out string seqId);// 平台流水号
                        resJSON.TryGetString("settleRefId", out string settleRefId);// 清分ID 如果来源方传了bankRefId就等于bankRefId，否则等于seqId
                        resJSON.TryGetValue("jsPayRequest", out JToken jsPayRequest);// JSAPI支付用的请求报文，带有签名信息
                        res.PayInfo = jsPayRequest.ToString();
                        channelRetMsg.ChannelOrderId = seqId;
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                        break;
                    case "00":
                    case "0000":
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            res.ChannelRetMsg = channelRetMsg;
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            AliBarOrderRQ bizRQ = (AliBarOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.AuthCode))
            {
                throw new BizException("用户支付条码[authCode]不可为空");
            }

            return null;
        }
    }
}
