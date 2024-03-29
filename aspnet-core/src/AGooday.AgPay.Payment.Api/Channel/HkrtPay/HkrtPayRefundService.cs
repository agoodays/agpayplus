using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.HkrtPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.HkrtPay
{
    /// <summary>
    /// 海科融通退款接口
    /// </summary>
    public class HkrtPayRefundService : AbstractRefundService
    {
        private readonly ILogger<HkrtPayRefundService> _logger;
        private readonly HkrtPayPaymentService hkrtPayPaymentService;
        public HkrtPayRefundService(ILogger<HkrtPayRefundService> logger,
            IServiceProvider serviceProvider,
            ConfigContextQueryService configContextQueryService,
            ISysConfigService sysConfigService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            _logger = logger;
            this.hkrtPayPaymentService = _serviceProvider.GetRequiredService<HkrtPayPaymentService>();
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.HKRTPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = HkrtPayEnum.GetTradeType(refundOrder.WayCode);
            string logPrefix = $"【海科融通({payType})退款查询】";
            try
            {
                reqParams.Add("out_refund_no", refundOrder.RefundOrderId); // 退款订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = hkrtPayPaymentService.PackageParamAndReq("/api/v1/pay/polymeric/refundquery", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string return_code = resJSON.GetValue("return_code").ToString(); //返回状态码
                resJSON.TryGetString("return_msg", out string return_msg); //返回错误信息
                if ("10000".Equals(return_code))
                {
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if (!string.IsNullOrWhiteSpace(error_code))
                    {
                        string status = resJSON.GetValue("refund_status").ToString();
                        string type = resJSON.GetValue("trade_type").ToString();
                        string refund_no = resJSON.GetValue("refund_no").ToString();//退款订单号 SaaS平台的退款订单编号
                        string channel_trade_no = resJSON.GetValue("channel_trade_no").ToString();//凭证条码订单号
                        var refundStatus = HkrtPayEnum.ConvertRefundStatus(status);
                        switch (refundStatus)
                        {
                            case HkrtPayEnum.RefundStatus.Success:
                                channelRetMsg.ChannelOrderId = refund_no;
                                var tradeType = HkrtPayEnum.ConvertTradeType(type);
                                var attach = hkrtPayPaymentService.GetHkrtAttach(resJSON);
                                attach.TryGetString("out_refund_no", out string out_refund_no);
                                channelRetMsg.PlatformMchOrderId = out_refund_no;
                                switch (tradeType)
                                {
                                    case HkrtPayEnum.TradeType.WX:
                                        attach.TryGetString("sub_openid", out string sub_openid);
                                        attach.TryGetString("refund_id", out string refund_id);
                                        channelRetMsg.ChannelUserId = sub_openid;
                                        channelRetMsg.PlatformOrderId = refund_id;
                                        break;
                                    case HkrtPayEnum.TradeType.ALI:
                                        attach.TryGetString("buyer_user_id", out string buyer_user_id);
                                        attach.TryGetString("trade_no", out string ali_trade_no);
                                        channelRetMsg.ChannelUserId = buyer_user_id;
                                        channelRetMsg.PlatformOrderId = ali_trade_no;
                                        break;
                                    case HkrtPayEnum.TradeType.UNIONQR:
                                        break;
                                }
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                _logger.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case HkrtPayEnum.RefundStatus.Failed:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = error_code;
                                channelRetMsg.ChannelErrMsg = error_msg;
                                _logger.LogInformation($"{logPrefix} >>> 退款失败, {error_msg}");
                                break;
                            case HkrtPayEnum.RefundStatus.Refunding:
                                //退款中
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                _logger.LogInformation($"{logPrefix} >>> 退款中");
                                break;
                        }
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
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = return_code;
                    channelRetMsg.ChannelErrMsg = return_msg;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }

        public override ChannelRetMsg Refund(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = HkrtPayEnum.GetTradeType(refundOrder.WayCode);
            string logPrefix = $"【海科融通({payType})订单退款】";
            try
            {
                reqParams.Add("out_refund_no", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("out_trade_no", payOrder.PayOrderId); // 原交易订单号
                reqParams.Add("refund_amount", AmountUtil.ConvertCent2Dollar(refundOrder.RefundAmount)); // 退款金额
                reqParams.Add("notify_url", GetNotifyUrl()); // 订单类型

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = hkrtPayPaymentService.PackageParamAndReq("/api/v1/pay/polymeric/refund", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string return_code = resJSON.GetValue("return_code").ToString(); //返回状态码
                resJSON.TryGetString("return_msg", out string return_msg); //返回错误信息
                if ("10000".Equals(return_code))
                {
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if (!string.IsNullOrWhiteSpace(error_code))
                    {
                        string status = resJSON.GetValue("refund_status").ToString();
                        string type = resJSON.GetValue("trade_type").ToString();
                        string refund_no = resJSON.GetValue("refund_no").ToString();//退款订单号 SaaS平台的退款订单编号
                        string channel_trade_no = resJSON.GetValue("channel_trade_no").ToString();//凭证条码订单号
                        var refundStatus = HkrtPayEnum.ConvertRefundStatus(status);
                        switch (refundStatus)
                        {
                            case HkrtPayEnum.RefundStatus.Success:
                                channelRetMsg.ChannelOrderId = refund_no;
                                var tradeType = HkrtPayEnum.ConvertTradeType(type);
                                var attach = hkrtPayPaymentService.GetHkrtAttach(resJSON);
                                attach.TryGetString("out_refund_no", out string out_refund_no);
                                channelRetMsg.PlatformMchOrderId = out_refund_no;
                                switch (tradeType)
                                {
                                    case HkrtPayEnum.TradeType.WX:
                                        attach.TryGetString("sub_openid", out string sub_openid);
                                        attach.TryGetString("refund_id", out string refund_id);
                                        channelRetMsg.ChannelUserId = sub_openid;
                                        channelRetMsg.PlatformOrderId = refund_id;
                                        break;
                                    case HkrtPayEnum.TradeType.ALI:
                                        attach.TryGetString("buyer_user_id", out string buyer_user_id);
                                        attach.TryGetString("trade_no", out string ali_trade_no);
                                        channelRetMsg.ChannelUserId = buyer_user_id;
                                        channelRetMsg.PlatformOrderId = ali_trade_no;
                                        break;
                                    case HkrtPayEnum.TradeType.UNIONQR:
                                        break;
                                }
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                _logger.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case HkrtPayEnum.RefundStatus.Failed:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = error_code;
                                channelRetMsg.ChannelErrMsg = error_msg;
                                _logger.LogInformation($"{logPrefix} >>> 退款失败, {error_msg}");
                                break;
                            case HkrtPayEnum.RefundStatus.Refunding:
                                //退款中
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                _logger.LogInformation($"{logPrefix} >>> 退款中");
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{logPrefix}, 异常:{e.Message}");
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }
    }
}
