﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.YsePay;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.YsePay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.YsePay.PayWay
{
    /// <summary>
    /// 银盛 微信jsapi
    /// </summary>
    public class WxJsapi : YsePayPaymentService
    {
        public WxJsapi(ILogger<WxJsapi> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【银盛(wechatJs)jsapi支付】";
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;

            //银盛扫一扫支付， 需要传入buyerUserId参数
            /*用户号（微信openid / 支付宝userid / 银联userid）
            payType == "WECHAT"或"ALIPAY"时必传*/
            reqParams.Add("sub_openid", bizRQ.Openid);
            // 获取微信官方配置的 appId
            reqParams.Add("appid", bizRQ.SubAppId);
            if (mchAppConfigContext.IsIsvSubMch())
            {
                YsePayIsvParams isvParams = (YsePayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.PartnerId == null)
                {
                    throw new BizException("服务商配置为空。");
                }
                reqParams.Add("business_code", isvParams.BusinessCode);
            }
            else
            {
                throw new BizException("不支持普通商户配置");
            }

            // 发送请求
            string method = "ysepay.online.weixin.pay", repMethod = "ysepay_online_weixin_pay_response";
            JObject resJSON = await PackageParamAndReqAsync(YsePayConfig.QRCODE_GATEWAY, method, repMethod, reqParams, GetNotifyUrl(), logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            var data = resJSON.GetValue(repMethod)?.ToObject<JObject>();
            string code = data?.GetValue("code").ToString();
            string msg = data?.GetValue("msg").ToString();
            data.TryGetString("sub_code", out string subCode);
            data.TryGetString("sub_msg", out string subMsg);
            channelRetMsg.ChannelMchNo = string.Empty;
            try
            {
                if ("10000".Equals(code))
                {
                    data.TryGetString("trade_no", out string yseTradeNo);//银盛支付交易流水号
                    data.TryGetString("channel_send_sn", out string channelSendSn);//发往渠道流水号
                    string tradeStatus = data.GetValue("trade_status").ToString();
                    var transStat = YsePayEnum.ConvertTradeStatus(tradeStatus);
                    switch (transStat)
                    {
                        case YsePayEnum.TradeStatus.WAIT_BUYER_PAY:
                        case YsePayEnum.TradeStatus.TRADE_PROCESS:
                        case YsePayEnum.TradeStatus.TRADE_ABNORMALITY:
                        case YsePayEnum.TradeStatus.TRADE_SUCCESS:
                            res.PayInfo = data.GetValue("jsapi_pay_info").ToString();
                            channelRetMsg.ChannelOrderId = yseTradeNo;
                            channelRetMsg.PlatformMchOrderId = channelSendSn;
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            break;
                        case YsePayEnum.TradeStatus.TRADE_FAILD:
                        case YsePayEnum.TradeStatus.TRADE_FAILED:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = subCode;
                            channelRetMsg.ChannelErrMsg = subMsg;
                            break;
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
                channelRetMsg.ChannelErrCode = subCode ?? code;
                channelRetMsg.ChannelErrMsg = subMsg ?? msg;
            }
            return res;
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[openId]不可为空");
            }

            return Task.FromResult<string>(null);
        }
    }
}
