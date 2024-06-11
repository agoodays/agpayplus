using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.YsePay;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.YsePay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.YsePay.PayWay
{
    /// <summary>
    /// 银盛 微信 小程序支付
    /// </summary>
    public class WxLite : YsePayPaymentService
    {
        public WxLite(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【银盛(wechat)小程序支付】";
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            WxLiteOrderRS res = ApiResBuilder.BuildSuccess<WxLiteOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            WxLiteOrderRQ bizRQ = (WxLiteOrderRQ)rq;

            // 获取微信官方配置的 appId
            YsePayIsvSubMchParams ysePayIsvParams = (YsePayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

            //银盛扫一扫支付， 需要传入buyerUserId参数
            /*用户号（微信openid / 支付宝userid / 银联userid）
            payType == "WECHAT"或"ALIPAY"时必传*/
            reqParams.Add("appid", ysePayIsvParams.SubMchLiteAppId);
            reqParams.Add("sub_openid", bizRQ.SubAppId);
            if (mchAppConfigContext.IsIsvSubMch())
            {
                YsePayIsvParams isvParams = (YsePayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

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
            JObject resJSON = PackageParamAndReq(YsePayConfig.QRCODE_GATEWAY, method, repMethod, reqParams, GetNotifyUrl(), logPrefix, mchAppConfigContext);
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
                            res.PayInfo = data.GetValue("pay_info").ToString();
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

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            WxLiteOrderRQ bizRQ = (WxLiteOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.Openid))
            {
                throw new BizException("[openId]不可为空");
            }

            return null;
        }
    }
}
