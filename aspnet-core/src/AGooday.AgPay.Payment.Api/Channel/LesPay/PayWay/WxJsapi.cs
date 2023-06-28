using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
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

namespace AGooday.AgPay.Payment.Api.Channel.LesPay.PayWay
{
    public class WxJsapi : LesPayPaymentService
    {
        /// <summary>
        /// 乐刷 微信jsapi
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
            string logPrefix = "【乐刷(wechatJs)jsapi支付】";
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl(), mchAppConfigContext);

            //微信JSAPI、微信小程序、支付宝JSAPI、支付宝小程序、银联JSAPI支付必填
            reqParams.Add("sub_openid", bizRQ.Openid);

            // 获取微信官方配置 的appId
            WxPayIsvParams wxpayIsvParams = (WxPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.WXPAY);
            reqParams.Add("appid", wxpayIsvParams.AppId); //用户ID

            // 发送请求
            JObject resJSON = PackageParamAndReq("/cgi-bin/lepos_pay_gateway.cgi", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string resp_code = resJSON.GetValue("resp_code").ToString(); //返回状态码
            resJSON.TryGetString("resp_msg", out string resp_msg); //返回错误信息
            try
            {
                if ("0".Equals(resp_code))
                {
                    string result_code = resJSON.GetValue("result_code").ToString(); //业务结果
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if ("0".Equals(result_code))
                    {
                        string leshua_order_id = resJSON.GetValue("leshua_order_id").ToString();//乐刷订单号
                        /*支付信息
                        原生公众号、服务窗、小程序，返回json格式字符串、银联JS支付返回URL，用云闪付打开此链接即可调起支付
                        生效时间:默认10分钟,具体时间看订单有效时间*/
                        resJSON.TryGetString("jspay_info", out string jspay_info);
                        resJSON.TryGetString("channel_order_id", out string channel_order_id);//通道订单号
                        res.PayInfo = jspay_info;
                        channelRetMsg.ChannelOrderId = channel_order_id;
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
                channelRetMsg.ChannelErrCode = resp_code;
                channelRetMsg.ChannelErrMsg = resp_msg;
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
