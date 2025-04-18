﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.SxfPay.PayWay
{
    /// <summary>
    /// 随行付 微信 小程序支付
    /// </summary>
    public class WxLite : SxfPayPaymentService
    {
        public WxLite(ILogger<WxLite> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【随行付(wechat)小程序支付】";
            JObject reqParams = new JObject();
            WxLiteOrderRS res = ApiResBuilder.BuildSuccess<WxLiteOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            WxLiteOrderRQ bizRQ = (WxLiteOrderRQ)rq;
            //随行付扫一扫支付， 需要传入buyerUserId参数
            /*用户号（微信openid / 支付宝userid / 银联userid）
            payType == "WECHAT"或"ALIPAY"时必传*/
            reqParams.Add("userId", bizRQ.GetChannelUserId());
            // 获取微信官方配置的 appId
            reqParams.Add("subAppId", bizRQ.SubAppId);

            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/order/jsapiScan", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON.GetValue("code").ToString(); //请求响应码
            string msg = resJSON.GetValue("msg").ToString(); //响应信息
            reqParams.TryGetString("mno", out string mno); // 商户号
            resJSON.TryGetString("orgId", out string orgId); //天阙平台机构编号
            channelRetMsg.ChannelMchNo = mno;
            channelRetMsg.ChannelIsvNo = orgId;
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
                        respData.TryGetString("prepayId", out string prepayId);//微信预下单id
                        string appId = respData.GetValue("payAppId").ToString();//微信 AppId
                        string timeStamp = respData.GetValue("payTimeStamp").ToString();//微信 TimeStamp
                        string nonceStr = respData.GetValue("paynonceStr").ToString();//微信 NonceStr
                        string package = respData.GetValue("payPackage").ToString();//微信 Package
                        string signType = respData.GetValue("paySignType").ToString();//微信 SignType
                        string paySign = respData.GetValue("paySign").ToString();//微信 Sign
                        respData.TryGetString("partnerId", out string partnerId);//微信 PartnerId
                        res.PayInfo = PayInfoBuilder.BuildPayInfoForLite(timeStamp, nonceStr, package, signType, paySign);
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
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = code;
                channelRetMsg.ChannelErrMsg = msg;
            }
            return res;
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxLiteOrderRQ bizRQ = (WxLiteOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[openId]不可为空");
            }

            return Task.FromResult<string>(null);
        }
    }
}
