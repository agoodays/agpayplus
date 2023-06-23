using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
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
    public class YsfJsapi : SxfPayPaymentService
    {
        /// <summary>
        /// 随行付 微信jsapi
        /// </summary>
        /// <param name="serviceProvider"></param>
        public YsfJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【随行付(unionpay)jsapi支付】";
            JObject reqParams = new JObject();
            YsfJsapiOrderRS res = ApiResBuilder.BuildSuccess<YsfJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            YsfJsapiOrderRQ bizRQ = (YsfJsapiOrderRQ)rq;

            // 请求参数赋值
            JsapiParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());
            //客户端IP
            /*持卡人ip地址
            银联js支付时，该参数必传*/
            reqParams.Add("customerIp", !string.IsNullOrWhiteSpace(payOrder.ClientIp) ? payOrder.ClientIp : "127.0.0.1");

            // 发送请求并返回订单状态
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
                        string redirectUrl = respData.GetValue("redirectUrl").ToString();//银联重定向跳转地址
                        res.RedirectUrl = redirectUrl;
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
            return null;
        }
    }
}
