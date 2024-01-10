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

namespace AGooday.AgPay.Payment.Api.Channel.LcswPay.PayWay
{
    /// <summary>
    /// 利楚扫呗 支付宝 jsapi
    /// </summary>
    public class AliJsapi : LcswPayPaymentService
    {
        public AliJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【利楚扫呗(alipayJs)jsapi支付】";
            AliJsapiOrderRQ bizRQ = (AliJsapiOrderRQ)rq;
            JObject reqParams = new JObject();
            AliJsapiOrderRS res = ApiResBuilder.BuildSuccess<AliJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl());

            // 利楚扫呗扫一扫支付， 需要传入buyerUserId参数
            reqParams.Add("open_id", bizRQ.BuyerUserId); // 用户标识（微信openid，支付宝userid），pay_type为010及020时必填

            // 发送请求
            JObject resJSON = PackageParamAndReq("/pay/open/jspay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
            string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
            reqParams.TryGetString("merchant_no", out string merchantNo); // 商户号
            channelRetMsg.ChannelMchNo = merchantNo;
            try
            {
                if ("01".Equals(returnCode))
                {
                    reqParams.TryGetString("result_code", out string resultCode); // 业务结果
                    if ("01".Equals(resultCode))
                    {
                        reqParams.TryGetString("out_trade_no", out string outTradeNo);// 平台唯一订单号
                        string aliTradeNo = reqParams.GetValue("ali_trade_no").ToString();// 支付宝JSAPI支付返回字段用于调起支付宝JSAPI
                        res.AlipayTradeNo = aliTradeNo;
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
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = returnCode;
                channelRetMsg.ChannelErrMsg = returnMsg;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            AliJsapiOrderRQ bizRQ = (AliJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.BuyerUserId))
            {
                throw new BizException("[buyerUserId]不可为空");
            }

            return null;
        }
    }
}
