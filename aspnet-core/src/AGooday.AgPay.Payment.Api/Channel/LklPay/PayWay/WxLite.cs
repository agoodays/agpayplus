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

namespace AGooday.AgPay.Payment.Api.Channel.LklPay.PayWay
{
    /// <summary>
    /// 拉卡拉 微信 小程序支付
    /// </summary>
    public class WxLite : LklPayPaymentService
    {
        public WxLite(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【拉卡拉(wechat)小程序支付】";
            JObject reqParams = new JObject();
            WxLiteOrderRS res = ApiResBuilder.BuildSuccess<WxLiteOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            WxLiteOrderRQ bizRQ = (WxLiteOrderRQ)rq;
            JObject accBusiFields = new JObject();
            //拉卡拉扫一扫支付， 需要传入buyerUserId参数
            /*用户号（微信openid / 支付宝userid / 银联userid）
            payType == "WECHAT"或"ALIPAY"时必传*/
            accBusiFields.Add("user_id", bizRQ.GetChannelUserId());

            //// 获取微信官方配置的 appId
            //LklPayIsvSubMchParams lklpayIsvParams = (LklPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
            accBusiFields.Add("sub_appid", bizRQ.SubAppId);
            reqParams.Add("acc_busi_fields", accBusiFields);

            // 发送请求
            JObject resJSON = PackageParamAndReq("/api/v3/labs/trans/preorder", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON?.GetValue("code").ToString(); //请求响应码
            string msg = resJSON?.GetValue("msg").ToString(); //响应信息
            try
            {
                if ("BBS00000".Equals(code))
                {
                    var respData = resJSON.GetValue("resp_data")?.ToObject<JObject>();
                    respData.TryGetString("merchant_no", out string merchantNo);//全局流水号
                    respData.TryGetString("trade_no", out string tradeNo);//全局流水号
                    var accRespFields = respData.GetValue("acc_resp_fields")?.ToObject<JObject>();
                    string prepayId = accRespFields.GetValue("prepay_id").ToString();//微信预下单id
                    string payAppId = accRespFields.GetValue("app_id").ToString();//微信 AppId
                    string payTimeStamp = accRespFields.GetValue("time_stamp").ToString();//微信 TimeStamp
                    string paynonceStr = accRespFields.GetValue("nonce_str").ToString();//微信 NonceStr
                    string payPackage = accRespFields.GetValue("package").ToString();//微信 Package
                    string paySignType = accRespFields.GetValue("sign_type").ToString();//微信 SignType
                    string paySign = accRespFields.GetValue("pay_sign").ToString();//微信 Sign
                    JObject payInfo = new JObject
                    {
                        { "appId", payAppId },
                        { "timeStamp", payTimeStamp },
                        { "nonceStr", paynonceStr },
                        { "package", payPackage },
                        { "signType", paySignType },
                        { "paySign", paySign }
                    };
                    res.PayInfo = payInfo.ToString();
                    channelRetMsg.ChannelMchNo = merchantNo;
                    channelRetMsg.ChannelOrderId = tradeNo;
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                }
                else if ("BBS11112".Equals(code) || "BBS11105".Equals(code) || "BBS10000".Equals(code))
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = code;
                channelRetMsg.ChannelErrMsg = msg;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            WxLiteOrderRQ bizRQ = (WxLiteOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[openId]不可为空");
            }

            return null;
        }
    }
}
