using AGooday.AgPay.Application.DataTransfer;
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

namespace AGooday.AgPay.Components.Third.Channel.LklPay.PayWay
{
    /// <summary>
    /// 拉卡拉 微信 小程序支付
    /// </summary>
    public class WxLite : LklPayPaymentService
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
            accBusiFields.Add("sub_appid", bizRQ.SubAppId);
            reqParams.Add("acc_busi_fields", accBusiFields);

            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/api/v3/labs/trans/preorder", reqParams, logPrefix, mchAppConfigContext);
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
                    accRespFields.TryGetString("prepay_id", out string prepayId);//微信预下单id
                    string appId = accRespFields.GetValue("app_id").ToString();//微信 AppId
                    string timeStamp = accRespFields.GetValue("time_stamp").ToString();//微信 TimeStamp
                    string nonceStr = accRespFields.GetValue("nonce_str").ToString();//微信 NonceStr
                    string package = accRespFields.GetValue("package").ToString();//微信 Package
                    string signType = accRespFields.GetValue("sign_type").ToString();//微信 SignType
                    string paySign = accRespFields.GetValue("pay_sign").ToString();//微信 Sign
                    res.PayInfo = PayInfoBuilder.BuildPayInfoForLite(timeStamp, nonceStr, package, signType, paySign);
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
