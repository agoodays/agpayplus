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

namespace AGooday.AgPay.Components.Third.Channel.LcswPay.PayWay
{
    /// <summary>
    /// 利楚扫呗 支付宝 jsapi
    /// </summary>
    public class AliJsapi : LcswPayPaymentService
    {
        public AliJsapi(ILogger<AliJsapi> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
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
            reqParams.Add("open_id", bizRQ.GetChannelUserId()); // 用户标识（微信openid，支付宝userid），pay_type为010及020时必填

            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/pay/open/jspay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
            string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
            resJSON.TryGetString("merchant_no", out string merchantNo); // 商户号
            channelRetMsg.ChannelMchNo = merchantNo;
            try
            {
                if ("01".Equals(returnCode))
                {
                    resJSON.TryGetString("result_code", out string resultCode); // 业务结果
                    if ("01".Equals(resultCode))
                    {
                        resJSON.TryGetString("out_trade_no", out string outTradeNo);// 平台唯一订单号
                        string aliTradeNo = resJSON.GetValue("ali_trade_no").ToString();// 支付宝JSAPI支付返回字段用于调起支付宝JSAPI
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
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = returnCode;
                channelRetMsg.ChannelErrMsg = returnMsg;
            }
            return res;
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AliJsapiOrderRQ bizRQ = (AliJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[buyerUserId]不可为空");
            }

            return Task.FromResult<string>(null);
        }
    }
}
