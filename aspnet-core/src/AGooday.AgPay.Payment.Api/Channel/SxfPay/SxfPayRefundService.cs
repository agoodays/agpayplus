using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.SxfPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay
{
    /// <summary>
    /// 随行付退款接口
    /// </summary>
    public class SxfPayRefundService : AbstractRefundService
    {
        private readonly ILogger<SxfPayRefundService> log;
        private readonly SxfPayPaymentService sxfpayPaymentService;
        public SxfPayRefundService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService,
            ILogger<SxfPayRefundService> log,
            SxfPayPaymentService sxfpayPaymentService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            this.log = log;
            this.sxfpayPaymentService = sxfpayPaymentService;
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.SXFPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = SxfPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【随行付({payType})退款查询】";
            try
            {
                reqParams.Add("ordNo", refundOrder.RefundOrderId); // 退款订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = sxfpayPaymentService.PackageParamAndReq("/query/refundQuery", reqParams, logPrefix, mchAppConfigContext);
                log.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON.GetValue("code").ToString(); //请求响应码
                string msg = resJSON.GetValue("msg").ToString(); //响应信息
                channelRetMsg.ChannelOrderId = refundOrder.RefundOrderId;
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
                        var orderStatus = SxfPayEnum.ConvertOrderStatus(tranSts);
                        switch (orderStatus)
                        {
                            case SxfPayEnum.OrderStatus.REFUNDSUC:
                                channelRetMsg.ChannelOrderId = uuid;
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                log.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case SxfPayEnum.OrderStatus.REFUNDFAIL:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = bizCode;
                                channelRetMsg.ChannelErrMsg = bizMsg;
                                log.LogInformation($"{logPrefix} >>> 退款失败, {bizMsg}");
                                break;
                            case SxfPayEnum.OrderStatus.REFUNDING:
                                //退款中
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                log.LogInformation($"{logPrefix} >>> 退款中");
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
            }
            catch (Exception e)
            {
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }

        public override ChannelRetMsg Refund(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = SxfPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【随行付({payType})订单退款】";
            try
            {
                reqParams.Add("ordNo", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("origOrderNo", payOrder.PayOrderId); // 原交易订单号
                reqParams.Add("amt", AmountUtil.ConvertCent2Dollar(refundOrder.RefundAmount)); // 退款金额
                reqParams.Add("notifyUrl", GetNotifyUrl()); // 订单类型
                reqParams.Add("refundReason", refundOrder.RefundReason); // 退货原因

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = sxfpayPaymentService.PackageParamAndReq("/order/refund", reqParams, logPrefix, mchAppConfigContext);
                log.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON.GetValue("code").ToString(); //请求响应码
                string msg = resJSON.GetValue("msg").ToString(); //响应信息
                channelRetMsg.ChannelOrderId = refundOrder.RefundOrderId;
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
                        var orderStatus = SxfPayEnum.ConvertOrderStatus(tranSts);
                        switch (orderStatus)
                        {
                            case SxfPayEnum.OrderStatus.REFUNDSUC:
                                channelRetMsg.ChannelOrderId = uuid;
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                log.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case SxfPayEnum.OrderStatus.REFUNDFAIL:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = bizCode;
                                channelRetMsg.ChannelErrMsg = bizMsg;
                                log.LogInformation($"{logPrefix} >>> 退款失败, {bizMsg}");
                                break;
                            case SxfPayEnum.OrderStatus.REFUNDING:
                                //退款中
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                log.LogInformation($"{logPrefix} >>> 退款中");
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e, $"{logPrefix}, 异常:{e.Message}");
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }
    }
}
