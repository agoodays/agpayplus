using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.SxfPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay
{
    /// <summary>
    /// 随行付查单
    /// </summary>
    public class SxfPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<SxfPayPayOrderQueryService> log;
        private readonly SxfPayPaymentService sxfpayPaymentService;

        public SxfPayPayOrderQueryService(ILogger<SxfPayPayOrderQueryService> log,
            SxfPayPaymentService sxfpayPaymentService)
        {
            this.log = log;
            this.sxfpayPaymentService = sxfpayPaymentService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.SXFPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string payType = SxfHttpUtil.GetPayType(payOrder.WayCode);
            string logPrefix = $"【随行付({payType})查单】";

            try
            {
                reqParams.Add("ordNo", payOrder.PayOrderId); //订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = sxfpayPaymentService.PackageParamAndReq("/query/tradeQuery", reqParams, logPrefix, mchAppConfigContext);
                log.LogInformation($"查询订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                var channelRetMsg = ChannelRetMsg.Waiting();
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON.GetValue("code").ToString(); //请求响应码
                string msg = resJSON.GetValue("msg").ToString(); //响应信息
                if ("0000".Equals(code))
                {
                    var respData = resJSON.GetValue("respData").ToObject<JObject>();
                    string bizCode = respData.GetValue("bizCode").ToString(); //业务响应码
                    string bizMsg = respData.GetValue("bizMsg").ToString(); //业务响应信息
                    if ("0000".Equals(bizCode))
                    {
                        /*订单状态
                        取值范围：
                        SUCCESS 交易成功
                        FAIL 交易失败
                        PAYING 支付中*/
                        string tranSts = respData.GetValue("tranSts").ToString();
                        string uuid = respData.GetValue("uuid").ToString();//天阙平台订单号
                        /*落单号
                        仅供退款使用
                        消费者账单中的条形码订单号*/
                        respData.TryGetString("sxfUuid", out string sxfUuid);
                        respData.TryGetString("channelId", out string channelId);//渠道商商户号
                        respData.TryGetString("transactionId", out string transactionId);//微信/支付宝流水号
                        /*买家用户号
                        支付宝渠道：买家支付宝用户号buyer_user_id
                        微信渠道：微信平台的sub_openid*/
                        respData.TryGetString("buyerId", out string buyerId);
                        switch (tranSts)
                        {
                            case "SUCCESS":
                                channelRetMsg = ChannelRetMsg.ConfirmSuccess(uuid);  //支付成功
                                channelRetMsg.ChannelOrderId = uuid;
                                channelRetMsg.ChannelUserId = buyerId;
                                channelRetMsg.PlatformOrderId = transactionId;
                                channelRetMsg.PlatformMchOrderId = sxfUuid;
                                break;
                            case "FAIL":
                                channelRetMsg = ChannelRetMsg.ConfirmFail(bizCode, bizMsg);
                                break;
                            case "PAYING":
                                break;
                            case "CLOSED":
                                break;
                            case "CANCELED":
                                break;
                        }
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
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
                }
                return channelRetMsg; //支付中
            }
            catch (Exception e)
            {
                log.LogError(e, $"查询订单 payorderId:{payOrder.PayOrderId}, 异常:{e.Message}");
                return ChannelRetMsg.Waiting(); //支付中
            }
        }
    }
}
