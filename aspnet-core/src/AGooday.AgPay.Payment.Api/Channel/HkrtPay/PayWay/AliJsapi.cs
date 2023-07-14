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

namespace AGooday.AgPay.Payment.Api.Channel.HkrtPay.PayWay
{
    /// <summary>
    /// 海科融通 支付宝 jsapi
    /// </summary>
    public class AliJsapi : HkrtPayPaymentService
    {
        public AliJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【海科融通(alipayJs)jsapi支付】";
            AliJsapiOrderRQ bizRQ = (AliJsapiOrderRQ)rq;
            JObject reqParams = new JObject();
            AliJsapiOrderRS res = ApiResBuilder.BuildSuccess<AliJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            //微信JSAPI、微信小程序、支付宝JSAPI、支付宝小程序、银联JSAPI支付必填
            reqParams.Add("userid", bizRQ.BuyerUserId);

            // 发送请求
            JObject resJSON = PackageParamAndReq("/api/v1/pay/polymeric/jsapipay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string return_code = resJSON.GetValue("return_code").ToString(); //返回状态码
            resJSON.TryGetString("return_msg", out string return_msg); //返回错误信息
            try
            {
                if ("10000".Equals(return_code))
                {
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if (!string.IsNullOrWhiteSpace(error_code))
                    {
                        string trade_no = resJSON.GetValue("trade_no").ToString();//交易订单号 SaaS平台的交易订单编号
                        string channel_trade_no = resJSON.GetValue("channel_trade_no").ToString();//凭证条码订单号 仅支付宝和微信会返回(v1.24增加)
                        resJSON.TryGetString("alipay_no", out string alipay_no);//预下单订单号 支付宝交易下单成功后会返回
                        res.AlipayTradeNo = alipay_no;
                        channelRetMsg.ChannelOrderId = trade_no;
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = error_code;
                        channelRetMsg.ChannelErrMsg = error_msg;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
            }
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = return_code;
                channelRetMsg.ChannelErrMsg = return_msg;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            AliJsapiOrderRQ bizRQ = (AliJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.BuyerUserId))
            {
                throw new BizException("[buyerUserId]不可为空");
            }

            return null;
        }
    }
}
