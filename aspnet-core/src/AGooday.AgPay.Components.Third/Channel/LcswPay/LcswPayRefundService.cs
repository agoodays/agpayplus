using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.LcswPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.LcswPay
{
    /// <summary>
    /// 利楚扫呗退款接口
    /// </summary>
    public class LcswPayRefundService : AbstractRefundService
    {
        private readonly LcswPayPaymentService _paymentService;

        public LcswPayRefundService(ILogger<LcswPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
            _paymentService = ActivatorUtilities.CreateInstance<LcswPayPaymentService>(serviceProvider);
        }

        public LcswPayRefundService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LCSWPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override async Task<ChannelRetMsg> QueryAsync(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = LcswPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【利楚扫呗({payType})退款查询】";
            try
            {
                reqParams.Add("pay_ver", "201");
                reqParams.Add("pay_type", payType);
                reqParams.Add("service_id", "031");
                reqParams.Add("terminal_trace", Guid.NewGuid().ToString("N"));
                reqParams.Add("terminal_time", DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (!string.IsNullOrWhiteSpace(refundOrder.ChannelOrderNo))
                {
                    reqParams.Add("out_refund_no", refundOrder.ChannelOrderNo);
                }
                else
                {
                    reqParams.Add("pay_trace", refundOrder.RefundOrderId);
                    reqParams.Add("pay_time", refundOrder.CreatedAt.Value.ToString("yyyyMMddHHmmss"));
                }

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/pay/open/queryrefund", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
                string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
                resJSON.TryGetString("merchant_no", out string merchantNo); // 商户号
                channelRetMsg.ChannelMchNo = merchantNo;
                if ("01".Equals(returnCode))
                {
                    resJSON.TryGetString("result_code", out string resultCode); // 业务结果
                    if ("01".Equals(resultCode))
                    {
                        resJSON.TryGetString("out_refund_no", out string outRefundNo);// 利楚唯一退款订单号
                        resJSON.TryGetString("channel_trade_no", out string channelTradeNo);// 微信/支付宝流水号
                        resJSON.TryGetString("channel_order_no", out string channelOrderNo);// 银行渠道订单号，微信支付时显示在支付成功页面的条码，可用作扫码查询和扫码退款时匹配
                        resJSON.TryGetString("user_id", out string userId);// 付款方用户id，服务商appid下的“微信openid”、“支付宝账户”
                        resJSON.TryGetString("trade_state", out string tradeState);
                        var _tradeState = LcswPayEnum.ConvertTradeState(tradeState);
                        switch (_tradeState)
                        {
                            case LcswPayEnum.TradeState.SUCCESS:
                                channelRetMsg.ChannelOrderId = outRefundNo;
                                channelRetMsg.ChannelUserId = userId;
                                channelRetMsg.PlatformOrderId = channelTradeNo;
                                channelRetMsg.PlatformMchOrderId = channelOrderNo;
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                _logger.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case LcswPayEnum.TradeState.FAIL:
                            case LcswPayEnum.TradeState.NOREFUND:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = resultCode;
                                channelRetMsg.ChannelErrMsg = returnMsg;
                                _logger.LogInformation($"{logPrefix} >>> 退款失败, {returnMsg}");
                                break;
                            case LcswPayEnum.TradeState.REFUNDING:
                                //退款中
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                _logger.LogInformation($"{logPrefix} >>> 退款中");
                                break;
                        }
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
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = returnCode;
                    channelRetMsg.ChannelErrMsg = returnMsg;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }

        public override async Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = LcswPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【利楚扫呗({payType})订单退款】";
            try
            {
                reqParams.Add("pay_ver", "201");
                reqParams.Add("pay_type", payType);
                reqParams.Add("service_id", "030");
                reqParams.Add("terminal_trace", refundOrder.RefundOrderId);
                reqParams.Add("terminal_time", refundOrder.CreatedAt.Value.ToString("yyyyMMddHHmmss"));
                reqParams.Add("refund_fee", refundOrder.RefundAmount); // 退款金额
                if (!string.IsNullOrWhiteSpace(payOrder.PlatformOrderNo))
                {
                    reqParams.Add("out_trade_no", payOrder.PlatformOrderNo);
                }
                else if (!string.IsNullOrWhiteSpace(payOrder.PlatformMchOrderNo))
                {
                    reqParams.Add("out_trade_no", payOrder.PlatformMchOrderNo);
                }
                else if (!string.IsNullOrWhiteSpace(payOrder.ChannelOrderNo))
                {
                    reqParams.Add("out_trade_no", payOrder.ChannelOrderNo);
                }
                else
                {
                    reqParams.Add("pay_trace", payOrder.PayOrderId);
                    reqParams.Add("pay_time", payOrder.CreatedAt.Value.ToString("yyyyMMddHHmmss"));
                }

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/pay/open/refund", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
                string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
                resJSON.TryGetString("merchant_no", out string merchantNo); // 商户号
                channelRetMsg.ChannelMchNo = merchantNo;
                if ("01".Equals(returnCode))
                {
                    resJSON.TryGetString("result_code", out string resultCode); // 业务结果
                    if ("01".Equals(resultCode))
                    {
                        resJSON.TryGetString("out_refund_no", out string outRefundNo);// 利楚唯一退款订单号
                        channelRetMsg.ChannelOrderId = outRefundNo;
                        channelRetMsg.ChannelState = ChannelState.WAITING; //退款中
                        _logger.LogInformation($"{logPrefix} >>> 退款中");
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
