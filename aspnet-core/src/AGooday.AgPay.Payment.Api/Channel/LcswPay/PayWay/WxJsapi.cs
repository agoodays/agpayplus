using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.LcswPay;
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

namespace AGooday.AgPay.Payment.Api.Channel.LcswPay.PayWay
{
    /// <summary>
    /// 利楚扫呗 微信jsapi
    /// </summary>
    public class WxJsapi : LcswPayPaymentService
    {
        public WxJsapi(ILogger<WxJsapi> logger, 
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【利楚扫呗(wechatJs)jsapi支付】";
            JObject reqParams = new JObject();
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl());

            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            reqParams.Add("open_id", bizRQ.GetChannelUserId());// 用户标识（微信openid，支付宝userid），pay_type为010及020时必填

            // 获取微信官方配置的 appId
            LcswPayNormalMchParams lcswParams = (LcswPayNormalMchParams)_configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            reqParams.Add("sub_appid", lcswParams.SubMchAppId); // 传商户自己的公众号appid，微信支付时此参数必传。（即获取open_id所使用的appid）。

            // 发送请求
            JObject resJSON = PackageParamAndReq("/pay/open/jspay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
            string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
            resJSON.TryGetString("merchant_no", out string merchantNo); // 商户号
            channelRetMsg.ChannelMchNo = merchantNo;
            try
            {
                if ("01".Equals(returnCode))
                {
                    resJSON.TryGetString("result_code", out string resultCode); // 业务结果
                    if ("01".Equals(resultCode))
                    {
                        resJSON.TryGetString("out_trade_no", out string outTradeNo);// 平台唯一订单号
                        string appId = resJSON.GetValue("appId").ToString();//微信 AppId
                        string timeStamp = resJSON.GetValue("timeStamp").ToString();//微信 TimeStamp
                        string nonceStr = resJSON.GetValue("nonceStr").ToString();//微信 NonceStr
                        string package = resJSON.GetValue("package_str").ToString();//微信 Package
                        string signType = resJSON.GetValue("signType").ToString();//微信 SignType
                        string paySign = resJSON.GetValue("paySign").ToString();//微信 Sign
                        JObject payInfo = new JObject
                        {
                            { "appId", appId },
                            { "timeStamp", timeStamp },
                            { "nonceStr", nonceStr },
                            { "package", package },
                            { "signType", signType },
                            { "paySign", paySign }
                        };
                        res.PayInfo = payInfo.ToString();
                        channelRetMsg.ChannelOrderId = outTradeNo;
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = resultCode;
                        channelRetMsg.ChannelErrMsg = returnMsg;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = returnCode;
                channelRetMsg.ChannelErrMsg = returnMsg;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[openId]不可为空");
            }

            return null;
        }
    }
}
