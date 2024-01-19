using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
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
    /// 银联商务 支付宝 jsapi
    /// </summary>
    public class AliJsapi : UmsPayPaymentService
    {
        public AliJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【银联商务(alipayJs)jsapi支付】";
            AliJsapiOrderRQ bizRQ = (AliJsapiOrderRQ)rq;
            // 构造函数响应数据
            AliJsapiOrderRS res = ApiResBuilder.BuildSuccess<AliJsapiOrderRS>();

            // 业务处理
            JObject reqParams = new JObject();
            // 支付宝用户标识或者云闪付用户标识 支付宝必传，云闪付userId和code必传其一
            reqParams.Add("userId", bizRQ.BuyerUserId);
            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/v1/netpay/trade/create", reqParams, logPrefix, mchAppConfigContext);
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
                        resJSON.TryGetString("targetOrderld", out string targetOrderld);// 第三方订单号
                        resJSON.TryGetValue("jsPayRequest", out JToken jsPayRequest);// JSAPI支付用的请求报文，带有签名信息
                        ((JObject)jsPayRequest).TryGetString("orderNo", out string orderNo);// 预下单订单号 支付宝交易下单成功后会返回
                        res.AlipayTradeNo = orderNo;
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
