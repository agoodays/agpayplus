using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Utils;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.Services;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;
using Newtonsoft.Json;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using System.Security.Cryptography;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay
{
    /// <summary>
    /// 微信 jsapi支付
    /// </summary>
    public class WxJsapi : WxPayPaymentService
    {
        public WxJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            var wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

            // 微信统一下单请求对象
            var request = new CreatePayUnifiedOrderRequest()
            {
                TradeType = "JSAPI",
                OutTradeNumber = payOrder.PayOrderId,// 商户订单号
                AppId = wxServiceWrapper.AppId,// 微信 AppId
                Body = payOrder.Subject,// 订单描述
                Detail = JsonConvert.DeserializeObject<CreatePayMicroPayRequest.Types.Detail>(payOrder.Body),
                FeeType = "CNY",
                TotalFee = Convert.ToInt32(payOrder.Amount),
                ClientIp = payOrder.ClientIp,
                NotifyUrl = GetNotifyUrl(payOrder.PayOrderId),
                //ProductId = Guid.NewGuid().ToString("N")
            };

            //订单分账， 将冻结商户资金。
            if (IsDivisionOrder(payOrder))
            {
                request.IsProfitSharing = true;
            }

            //放置isv信息
            if (mchAppConfigContext.IsIsvSubMch())
            {
                var isvSubMchParams = (WxPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                request.SubMerchantId = isvSubMchParams.SubMchId;
                // 子商户subAppId不为空
                if (!string.IsNullOrEmpty(isvSubMchParams.SubMchAppId))
                {
                    request.SubAppId = isvSubMchParams.SubMchAppId;
                }
            }

            // 构造函数响应数据
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 调起上游接口：
            // 1. 如果抛异常，则订单状态为： 生成状态，此时没有查单处理操作。 订单将超时关闭
            // 2. 接口调用成功， 后续异常需进行捕捉， 如果 逻辑代码出现异常则需要走完正常流程，此时订单状态为： 支付中， 需要查单处理。
            var response = ((WechatTenpayClient)wxServiceWrapper.Client).ExecuteCreatePayUnifiedOrderAsync(request).Result;
            if (response.IsSuccessful())
            {
                var payInfo = new Dictionary<string, string>();
                payInfo.Add("appId", response.AppId);
                payInfo.Add("timeStamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                payInfo.Add("nonceStr", Guid.NewGuid().ToString("N"));
                payInfo.Add("package", $"prepay_id={response.PrepayId}");
                payInfo.Add("signType", "MD5");
                var paySign = Sign(payInfo, wxServiceWrapper.MchKey);
                payInfo.Add("paySign", paySign);
                res.PayInfo = JsonConvert.SerializeObject(payInfo);
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = WxPayKit.AppendErrCode(response.ReturnCode, response.ErrorCode); //优先： subCode
                var msg = "OK".Equals(response.ReturnMessage, StringComparison.CurrentCultureIgnoreCase) ? null : response.ReturnMessage;
                var subMsg = response.ErrorCodeDescription;
                channelRetMsg.ChannelErrMsg = WxPayKit.AppendErrMsg(subMsg, msg);
            }

            return res;
        }
        private string Sign(Dictionary<string, string> dictionary, string key)
        {
            var json = string.Join("&", dictionary.OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}")) + "&key=" + key;
            var bytes = Encoding.UTF8.GetBytes(json);
            MD5 md5 = MD5.Create();
            byte[] temp = md5.ComputeHash(bytes);
            String sign = "";
            foreach (byte b in temp)
            {
                sign = sign + b.ToString("X").PadLeft(2, '0');
            }
            return sign.ToUpper();
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.Openid))
            {
                throw new BizException("[openid]不可为空");
            }

            return null;
        }
    }
}
