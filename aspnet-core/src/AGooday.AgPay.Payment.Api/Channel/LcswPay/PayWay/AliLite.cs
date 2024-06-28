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
    /// 利楚扫呗 支付宝 小程序支付
    /// </summary>
    public class AliLite : LcswPayPaymentService
    {
        public AliLite(ILogger<AliLite> logger, 
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【利楚扫呗(alipay)小程序支付】";
            AliLiteOrderRQ bizRQ = (AliLiteOrderRQ)rq;
            JObject reqParams = new JObject();
            AliLiteOrderRS res = ApiResBuilder.BuildSuccess<AliLiteOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            MiniPayParamsSet(reqParams, payOrder, GetNotifyUrl());

            //利楚扫呗扫一扫支付， 需要传入buyerUserId参数
            reqParams.Add("open_id", bizRQ.GetChannelUserId()); // 用户标识（微信openid，支付宝userid），pay_type为010及020时必填

            // 发送请求
            JObject resJSON = PackageParamAndReq("/pay/open/minipay", reqParams, logPrefix, mchAppConfigContext);
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
                        string aliTradeNo = resJSON.GetValue("ali_trade_no").ToString();// 支付宝交易号
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

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            AliLiteOrderRQ bizRQ = (AliLiteOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[buyerUserId]不可为空");
            }

            return null;
        }
    }
}
