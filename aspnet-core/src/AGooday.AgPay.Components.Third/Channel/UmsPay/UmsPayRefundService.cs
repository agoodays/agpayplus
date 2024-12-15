using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.UmsPay
{
    /// <summary>
    /// 银联商务退款
    /// </summary>
    public class UmsPayRefundService : AbstractRefundService
    {
        private readonly UmsPayPaymentService _paymentService;

        public UmsPayRefundService(ILogger<UmsPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
            _paymentService = ActivatorUtilities.CreateInstance<UmsPayPaymentService>(serviceProvider);
        }

        public UmsPayRefundService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.UMSPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override async Task<ChannelRetMsg> QueryAsync(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = $"【银联商务({refundOrder.WayCode})退款查询】";

            try
            {
                var channelRetMsg = ChannelRetMsg.Waiting(); //退款中
                switch (refundOrder.WayCode)
                {
                    case CS.PAY_WAY_CODE.ALI_BAR:
                    case CS.PAY_WAY_CODE.WX_BAR:
                    case CS.PAY_WAY_CODE.YSF_BAR:
                        await BarQueryAsync(logPrefix, channelRetMsg, refundOrder, mchAppConfigContext);
                        break;
                    case CS.PAY_WAY_CODE.ALI_QR:
                        await QrQueryAsync(logPrefix, channelRetMsg, refundOrder, mchAppConfigContext);
                        break;
                    case CS.PAY_WAY_CODE.ALI_JSAPI:
                    case CS.PAY_WAY_CODE.WX_JSAPI:
                    case CS.PAY_WAY_CODE.YSF_JSAPI:
                        await UnifiedQueryAsync(logPrefix, channelRetMsg, refundOrder, mchAppConfigContext);
                        break;
                    default:
                        break;
                }
                return channelRetMsg;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 异常:{e.Message}");
                return ChannelRetMsg.Waiting(); //退款中
            }
        }

        private async Task<ChannelRetMsg> BarQueryAsync(string logPrefix, ChannelRetMsg channelRetMsg, RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("merchantOrderId", refundOrder.PayOrderId); // 商户订单号 商户订单号与银商订单号至少存在一个，如均存在，以银商订单号为准，忽略商户订单号	
            //reqParams.Add("originalOrderId", refundOrder.ChannelPayOrderNo); // 银商订单号 必须与原支付交易返回的订单号一致
            reqParams.Add("refundRequestId", refundOrder.RefundOrderId); // 退款订单号 多次退款必传，每次退款上送的refundOrderId值需不同

            // 封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = await _paymentService.PackageParamAndReqAsync("/v6/poslink/transaction/query-refund", reqParams, logPrefix, mchAppConfigContext, true);

            // 请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            string errInfo = resJSON.GetValue("errInfo").ToString(); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "00":
                    case "0000":
                    case "SUCCESS":
                        // 查询结果 0：成功 1：超时 2：已撤销 3：已退货 4：已冲正 5：失败（失败情况，后面追加失败描述) FF：交易状态未知
                        resJSON.TryGetString("queryResCode", out string queryResCode);
                        resJSON.TryGetString("queryResInfo", out string queryResInfo);
                        if (queryResCode.Equals("00"))
                        {
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        }
                        else
                        {
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = queryResCode;
                            channelRetMsg.ChannelErrMsg = queryResInfo;
                        }
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }

        private async Task<ChannelRetMsg> QrQueryAsync(string logPrefix, ChannelRetMsg channelRetMsg, RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));// 报文请求时间 格式：yyyy-MM-dd HH:mm:ss
            reqParams.Add("instMid", "QRPAYDEFAULT");// 业务类型 QRPAYDEFAULT
            reqParams.Add("billNo", refundOrder.PayOrderId); // 账单号
            // 封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = await _paymentService.PackageParamAndReqAsync("/v1/netpay/bills/query", reqParams, logPrefix, mchAppConfigContext);

            // 请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            string errInfo = resJSON.GetValue("errInfo").ToString(); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("billStatus", out string billStatus);// 账单状态
                        if (billStatus.Equals("REFUND"))
                        {
                            resJSON.TryGetValue("billPayment", out JToken billPayment); // 账单支付信息
                            ((JObject)billPayment).TryGetString("status", out string status);// 交易状态
                            if (status.Equals("TRADE_REFUND"))
                            {
                                ((JObject)billPayment).TryGetString("paySeqId", out string paySeqId);// 交易参考号
                                ((JObject)billPayment).TryGetString("buyerId", out string buyerId);// 交易参考号
                                ((JObject)billPayment).TryGetString("targetOrderId", out string targetOrderId);// 目标平台单号
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                channelRetMsg.ChannelOrderId = paySeqId;
                                channelRetMsg.ChannelUserId = buyerId;
                                channelRetMsg.PlatformOrderId = targetOrderId;
                            }
                        }
                        break;
                    case "00":
                    case "0000":
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }

        private async Task<ChannelRetMsg> UnifiedQueryAsync(string logPrefix, ChannelRetMsg channelRetMsg, RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));// 报文请求时间 格式：yyyy-MM-dd HH:mm:ss
            reqParams.Add("instMid", "YUEDANDEFAULT");// 业务类型 YUEDANDEFAULT
            reqParams.Add("merOrderId", refundOrder.RefundOrderId); // 商户订单号 原交易订单号
            // 封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = await _paymentService.PackageParamAndReqAsync("/v1/netpay/refund-query", reqParams, logPrefix, mchAppConfigContext);

            // 请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            string errInfo = resJSON.GetValue("errInfo").ToString(); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("refundStatus", out string refundStatus);// 账单状态
                        switch (refundStatus)
                        {
                            case "SUCCESS":
                                resJSON.TryGetString("refundOrderId", out string refundOrderId);// 退货订单号
                                resJSON.TryGetString("refundTargetOrderId", out string refundTargetOrderId);// 目标系统退货订单号
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                channelRetMsg.ChannelOrderId = refundOrderId;
                                channelRetMsg.PlatformOrderId = refundTargetOrderId;
                                break;
                            case "UNKNOWN":
                            case "PROCESSING":
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                break;
                            case "FAIL":
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = errCode;
                                channelRetMsg.ChannelErrMsg = errInfo;
                                break;
                        }
                        break;
                    case "0000":
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }

        public override async Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = $"【银联商务({payOrder.WayCode})订单退款】";

            try
            {
                var channelRetMsg = ChannelRetMsg.Waiting(); //退款中
                switch (payOrder.WayCode)
                {
                    case CS.PAY_WAY_CODE.ALI_BAR:
                    case CS.PAY_WAY_CODE.WX_BAR:
                    case CS.PAY_WAY_CODE.YSF_BAR:
                        await BarRefundAsync(logPrefix, channelRetMsg, bizRQ, refundOrder, payOrder, mchAppConfigContext);
                        break;
                    case CS.PAY_WAY_CODE.ALI_QR:
                        await QrRefundAsync(logPrefix, channelRetMsg, bizRQ, refundOrder, payOrder, mchAppConfigContext);
                        break;
                    case CS.PAY_WAY_CODE.ALI_JSAPI:
                    case CS.PAY_WAY_CODE.WX_JSAPI:
                    case CS.PAY_WAY_CODE.YSF_JSAPI:
                        await UnifiedRefundAsync(logPrefix, channelRetMsg, bizRQ, refundOrder, payOrder, mchAppConfigContext);
                        break;
                    default:
                        break;
                }
                return channelRetMsg;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{logPrefix}, 异常:{e.Message}");
                return ChannelRetMsg.Waiting(); //支付中
            }
        }

        private async Task<ChannelRetMsg> BarRefundAsync(string logPrefix, ChannelRetMsg channelRetMsg, RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("merchantOrderId", payOrder.PayOrderId); // 商户订单号 商户订单号与银商订单号至少存在一个，如均存在，以银商订单号为准，忽略商户订单号	
            //reqParams.Add("originalOrderId", payOrder.ChannelOrderNo); // 银商订单号 必须与原支付交易返回的订单号一致
            reqParams.Add("refundRequestId", refundOrder.RefundOrderId); // 退款订单号 多次退款必传，每次退款上送的refundOrderId值需不同
            reqParams.Add("transactionAmount", refundOrder.RefundAmount); // 要退货的金额，单位：分
            reqParams.Add("refundDesc", refundOrder.RefundReason); // 退货说明
            reqParams.Add("transactionCurrencyCode", "156"); // 交易币种 必须与原支付交易一致
            // 封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = await _paymentService.PackageParamAndReqAsync("/v6/poslink/transaction/refund", reqParams, logPrefix, mchAppConfigContext, true);

            // 请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            string errInfo = resJSON.GetValue("errInfo").ToString(); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "00":
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        break;
                    case "0000":
                    case "SUCCESS":
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }

        private async Task<ChannelRetMsg> QrRefundAsync(string logPrefix, ChannelRetMsg channelRetMsg, RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));// 报文请求时间 格式：yyyy-MM-dd HH:mm:ss
            reqParams.Add("instMid", "QRPAYDEFAULT");// 业务类型 QRPAYDEFAULT
            reqParams.Add("billNo", payOrder.PayOrderId); // 账单号
            reqParams.Add("billDate", payOrder.CreatedAt?.ToString("yyyy-MM-dd")); // 订单时间 格式：yyyy-MM-dd
            reqParams.Add("refundOrderId", refundOrder.RefundOrderId); // 退款订单号 多次退款必传，每次退款上送的refundOrderId值需不同
            reqParams.Add("refundAmount", refundOrder.RefundAmount); // 要退货的金额，单位：分
            // 封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = await _paymentService.PackageParamAndReqAsync("/v1/netpay/bills/refund", reqParams, logPrefix, mchAppConfigContext);

            // 请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            string errInfo = resJSON.GetValue("errInfo").ToString(); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("billStatus", out string billStatus);// 账单状态
                        if (billStatus.Equals("REFUND"))
                        {
                            resJSON.TryGetString("refundStatus", out string refundStatus);// 账单状态
                            switch (refundStatus)
                            {
                                case "SUCCESS":
                                    resJSON.TryGetString("refundTargetOrderId", out string refundTargetOrderId);// 目标系统退货订单号
                                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                    channelRetMsg.PlatformOrderId = refundTargetOrderId;
                                    break;
                                case "UNKNOWN":
                                case "PROCESSING":
                                    channelRetMsg.ChannelState = ChannelState.WAITING;
                                    break;
                                case "FAIL":
                                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                    channelRetMsg.ChannelErrCode = errCode;
                                    channelRetMsg.ChannelErrMsg = errInfo;
                                    break;
                            }
                        }
                        break;
                    case "00":
                    case "0000":
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }

        private async Task<ChannelRetMsg> UnifiedRefundAsync(string logPrefix, ChannelRetMsg channelRetMsg, RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));// 报文请求时间 格式：yyyy-MM-dd HH:mm:ss
            reqParams.Add("instMid", "YUEDANDEFAULT");// 业务类型 YUEDANDEFAULT
            reqParams.Add("merOrderId", payOrder.PayOrderId); // 商户订单号 原交易订单号
            reqParams.Add("refundAmount", refundOrder.RefundAmount); // 要退货的金额 若下单接口中上送了分账标记字段divisionFlag，则该字段refundAmount=subOrders中totalAmount之和+platformAmount
            reqParams.Add("refundOrderId", refundOrder.RefundOrderId); // 退款订单号 多次退款必传，每次退款上送的refundOrderId值需不同，若多次退货，且后续退货上送的merOrderId和refundOrderId字段与之前退货上送的值一致，将不会走退货逻辑，而是返回已有退货订单的退货信息，遵循商户订单号生成规范
            reqParams.Add("refundDesc", refundOrder.RefundReason); // 退货说明
            // 封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = await _paymentService.PackageParamAndReqAsync("/v1/netpay/refund", reqParams, logPrefix, mchAppConfigContext);

            // 请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            string errInfo = resJSON.GetValue("errInfo").ToString(); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("status", out string status);// 账单状态
                        if (status.Equals("REFUND"))
                        {
                            resJSON.TryGetString("refundStatus", out string refundStatus);// 账单状态
                            switch (refundStatus)
                            {
                                case "SUCCESS":
                                    resJSON.TryGetString("refundOrderId", out string refundOrderId);// 退货订单号
                                    resJSON.TryGetString("refundTargetOrderId", out string refundTargetOrderId);// 目标系统退货订单号
                                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                    channelRetMsg.ChannelOrderId = refundOrderId;
                                    channelRetMsg.PlatformOrderId = refundTargetOrderId;
                                    break;
                                case "UNKNOWN":
                                case "PROCESSING":
                                    channelRetMsg.ChannelState = ChannelState.WAITING;
                                    break;
                                case "FAIL":
                                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                    channelRetMsg.ChannelErrCode = errCode;
                                    channelRetMsg.ChannelErrMsg = errInfo;
                                    break;
                            }
                        }
                        break;
                    case "00":
                    case "0000":
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                    default:
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = errCode;
                        channelRetMsg.ChannelErrMsg = errInfo;
                        break;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }
    }
}
