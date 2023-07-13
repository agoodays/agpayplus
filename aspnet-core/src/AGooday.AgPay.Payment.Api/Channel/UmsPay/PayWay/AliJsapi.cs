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
    public class AliJsapi : UmsPayPaymentService
    {
        /// <summary>
        /// 银联商务 支付宝 条码支付
        /// </summary>
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
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            reqParams.Add("merOrderId", payOrder.PayOrderId);
            reqParams.Add("instMid", "YUEDANDEFAULT");
            reqParams.Add("tradeType", "JSAPI");
            // 支付宝用户标识或者云闪付用户标识 支付宝必传，云闪付userId和code必传其一
            reqParams.Add("userId", bizRQ.BuyerUserId);
            reqParams.Add("originalAmount", payOrder.Amount);
            reqParams.Add("totalAmount", payOrder.Amount);
            reqParams.Add("notifyUrl", GetNotifyUrl());
            reqParams.Add("returnUrl", GetReturnUrl());
            reqParams.Add("clientIp", payOrder.ClientIp);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/v1/netpay/trade/create", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            string errInfo = resJSON.GetValue("errInfo").ToString(); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "TRADE_SUCCESS":
                        resJSON.TryGetString("targetOrderId", out string targetOrderId); //错误码
                        channelRetMsg.ChannelOrderId = targetOrderId;
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        break;
                    case "0000":
                    case "00":
                    case "SUCCESS":
                    default:
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                        break;
                }
            }
            catch (Exception e)
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
