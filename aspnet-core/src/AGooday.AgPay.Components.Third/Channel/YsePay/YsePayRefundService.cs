using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.YsePay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.YsePay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.YsePay
{
    /// <summary>
    /// 银盛退款接口
    /// </summary>
    public class YsePayRefundService : AbstractRefundService
    {
        private readonly YsePayPaymentService ysePayPaymentService;
        public YsePayRefundService(ILogger<YsePayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
            this.ysePayPaymentService = ActivatorUtilities.CreateInstance<YsePayPaymentService>(serviceProvider);
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSEPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            string logPrefix = $"【银盛({refundOrder.WayCode})退款查询】";
            try
            {
                reqParams.Add("out_trade_no", refundOrder.PayOrderId);
                reqParams.Add("trade_no", refundOrder.ChannelPayOrderNo);
                reqParams.Add("out_request_no", refundOrder.RefundOrderId);

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                string method = "ysepay.online.trade.refund.query", repMethod = "ysepay_online_trade_refund_query_response";
                JObject resJSON = ysePayPaymentService.PackageParamAndReq(YsePayConfig.SEARCH_GATEWAY, method, repMethod, reqParams, string.Empty, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                var data = resJSON.GetValue(repMethod)?.ToObject<JObject>();
                string code = data?.GetValue("code").ToString();
                string msg = data?.GetValue("msg").ToString();
                data.TryGetString("sub_code", out string subCode);
                data.TryGetString("sub_msg", out string subMsg);
                channelRetMsg.ChannelMchNo = string.Empty;
                if ("10000".Equals(code))
                {
                    data.TryGetString("trade_no", out string tradeNo);//银盛支付交易流水号
                    data.TryGetString("funds_dynamics", out string fundsDynamics);//银盛支付交易流水号
                    var refundChannelfundsDynamicDto = fundsDynamics.ToObject<JObject>();
                    refundChannelfundsDynamicDto.TryGetString("channelRecvSn", out string channelRecvSn);//渠道返回流水号	
                    refundChannelfundsDynamicDto.TryGetString("channelSendSn", out string channelSendSn);//发往渠道流水号
                    string _refundState = data.GetValue("refund_state").ToString();
                    var refundState = YsePayEnum.ConvertRefundState(_refundState);
                    switch (refundState)
                    {
                        case YsePayEnum.RefundState.success:
                            //退款成功
                            channelRetMsg.ChannelOrderId = tradeNo;
                            channelRetMsg.PlatformOrderId = channelRecvSn;
                            channelRetMsg.PlatformMchOrderId = channelSendSn;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            _logger.LogInformation($"{logPrefix} >>> 退款成功");
                            break;
                        case YsePayEnum.RefundState.in_process:
                            //退款中
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            _logger.LogInformation($"{logPrefix} >>> 退款中");
                            break;
                        case YsePayEnum.RefundState.fail:
                            //明确退款失败
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = subCode;
                            channelRetMsg.ChannelErrMsg = subMsg;
                            _logger.LogInformation($"{logPrefix} >>> 退款失败, {subMsg}");
                            break;
                    }
                }
                else if ("50000".Equals(code) || "3501".Equals(code))
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = subCode ?? code;
                    channelRetMsg.ChannelErrMsg = subMsg ?? msg;
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
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            string logPrefix = $"【银盛({refundOrder.WayCode})订单退款】";
            try
            {
                reqParams.Add("out_request_no", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("refund_amount", AmountUtil.ConvertCent2Dollar(refundOrder.RefundAmount)); // 退款金额
                reqParams.Add("shopdate", payOrder.CreatedAt.Value.ToString("yyyyMMdd")); // 原交易订单号
                reqParams.Add("out_trade_no", payOrder.PayOrderId); // 原交易订单号
                reqParams.Add("trade_no", payOrder.ChannelOrderNo); // 原交易订单号
                reqParams.Add("refund_reason", refundOrder.RefundReason); // 退货原因

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                string method = "ysepay.online.trade.refund", repMethod = "ysepay_online_trade_refund_response";
                JObject resJSON = ysePayPaymentService.PackageParamAndReq(YsePayConfig.OPENAPI_GATEWAY, method, repMethod, reqParams, GetNotifyUrl(), logPrefix, mchAppConfigContext);
                _logger.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }

                //请求 & 响应成功， 判断业务逻辑
                var data = resJSON.GetValue(repMethod)?.ToObject<JObject>();
                string code = data?.GetValue("code").ToString();
                string msg = data?.GetValue("msg").ToString();
                data.TryGetString("sub_code", out string subCode);
                data.TryGetString("sub_msg", out string subMsg);
                channelRetMsg.ChannelMchNo = string.Empty;
                if ("10000".Equals(code))
                {
                    data.TryGetString("refundsn", out string refundsn);//银盛退款流水号
                    //退款成功
                    channelRetMsg.ChannelOrderId = refundsn;
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    _logger.LogInformation($"{logPrefix} >>> 退款成功");
                }
                else if ("50000".Equals(code) || "3501".Equals(code))
                {
                    //退款中
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    _logger.LogInformation($"{logPrefix} >>> 退款中");
                }
                else
                {
                    //明确退款失败
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = subCode ?? code;
                    channelRetMsg.ChannelErrMsg = subMsg ?? msg;
                    _logger.LogInformation($"{logPrefix} >>> 退款失败, {subMsg}");
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
