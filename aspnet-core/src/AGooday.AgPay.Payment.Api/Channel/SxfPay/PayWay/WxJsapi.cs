using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay.PayWay
{
    public class WxJsapi : SxfPayPaymentService
    {
        /// <summary>
        /// 随行付 微信jsapi
        /// </summary>
        /// <param name="serviceProvider"></param>
        public WxJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【随行付(wechatJs)jsapi支付】";
            JObject reqParams = new JObject();
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            JsapiParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            //随行付扫一扫支付， 需要传入buyerUserId参数
            /*用户号（微信openid / 支付宝userid / 银联userid）
            payType == "WECHAT"或"ALIPAY"时必传*/
            reqParams.Add("userId", bizRQ.Openid);

            // 获取微信官方配置 的appId
            WxPayIsvParams wxpayIsvParams = (WxPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.WXPAY);
            reqParams.Add("subAppId", wxpayIsvParams.AppId); //用户ID

            // 发送请求
            JObject resJSON = PackageParamAndReq("/order/jsapiScan", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON.GetValue("code").ToString(); //请求响应码
            string msg = resJSON.GetValue("msg").ToString(); //响应信息
            try
            {
                if ("0000".Equals(code))
                {
                    var respData = resJSON.GetValue("respData").ToObject<JObject>();
                    string bizCode = respData.GetValue("bizCode").ToString(); //业务响应码
                    string bizMsg = respData.GetValue("bizMsg").ToString(); //业务响应信息
                    if ("0000".Equals(bizCode))
                    {
                        string uuid = respData.GetValue("uuid").ToString();//天阙平台订单号
                        /*落单号
                        仅供退款使用
                        消费者账单中的条形码订单号*/
                        string sxfUuid = respData.GetValue("sxfUuid").ToString();
                        string prepayId = respData.GetValue("prepayId").ToString();//微信预下单id
                        string payAppId = respData.GetValue("payAppId").ToString();//微信 AppId
                        string payTimeStamp = respData.GetValue("payTimeStamp").ToString();//微信 TimeStamp
                        string paynonceStr = respData.GetValue("paynonceStr").ToString();//微信 NonceStr
                        string payPackage = respData.GetValue("payPackage").ToString();//微信 Package
                        string paySignType = respData.GetValue("paySignType").ToString();//微信 SignType
                        string paySign = respData.GetValue("paySign").ToString();//微信 Sign
                        string partnerId = respData.GetValue("partnerId").ToString();//微信 PartnerId
                        JObject payInfo = new JObject
                        {
                            { "prepayId", prepayId },
                            { "payAppId", payAppId },
                            { "payTimeStamp", payTimeStamp },
                            { "paynonceStr", paynonceStr },
                            { "payPackage", payPackage },
                            { "paySignType", paySignType },
                            { "paySign", paySign },
                            { "partnerId", partnerId }
                        };
                        res.PayInfo = payInfo.ToString();
                        channelRetMsg.ChannelOrderId = uuid;
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = bizCode;
                        channelRetMsg.ChannelErrMsg = bizMsg;
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
                channelRetMsg.ChannelErrCode = code;
                channelRetMsg.ChannelErrMsg = msg;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.Openid))
            {
                throw new BizException("[openId]不可为空");
            }

            return null;
        }
    }
}
