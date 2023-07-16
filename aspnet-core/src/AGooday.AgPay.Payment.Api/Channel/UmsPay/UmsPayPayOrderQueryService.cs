using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay
{
    /// <summary>
    /// 银联商务查单
    /// </summary>
    public class UmsPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<UmsPayPayOrderQueryService> log;
        private readonly UmsPayPaymentService umsPayPaymentService;

        public UmsPayPayOrderQueryService(ILogger<UmsPayPayOrderQueryService> log,
            UmsPayPaymentService umsPayPaymentService)
        {
            this.log = log;
            this.umsPayPaymentService = umsPayPaymentService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.UMSPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string logPrefix = $"【银联商务({payOrder.WayCode})查单】";

            try
            {
                var channelRetMsg = ChannelRetMsg.Waiting(); //支付中
                switch (payOrder.WayCode)
                {
                    case CS.PAY_WAY_CODE.ALI_BAR:
                    case CS.PAY_WAY_CODE.WX_BAR:
                    case CS.PAY_WAY_CODE.YSF_BAR:
                        BarQuery(logPrefix, channelRetMsg, payOrder, mchAppConfigContext);
                        break;
                    case CS.PAY_WAY_CODE.ALI_QR:
                        QrQuery(logPrefix, channelRetMsg, payOrder, mchAppConfigContext);
                        break;
                    case CS.PAY_WAY_CODE.ALI_JSAPI:
                    case CS.PAY_WAY_CODE.WX_JSAPI:
                    case CS.PAY_WAY_CODE.YSF_JSAPI:
                        UnifiedQuery(logPrefix, channelRetMsg, payOrder, mchAppConfigContext);
                        break;
                    default:
                        break;
                }
                return channelRetMsg;
            }
            catch (Exception e)
            {
                log.LogError(e, $"查询订单 payorderId:{payOrder.PayOrderId}, 异常:{e.Message}");
                return ChannelRetMsg.Waiting(); //支付中
            }
        }

        private ChannelRetMsg BarQuery(string logPrefix, ChannelRetMsg channelRetMsg, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("merchantOrderId", payOrder.PayOrderId); // 商户订单号
            //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = umsPayPaymentService.PackageParamAndReq("/v6/poslink/transaction/query", reqParams, logPrefix, mchAppConfigContext, true);

            //请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            resJSON.TryGetString("errInfo", out string errInfo); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("orderId", out string orderId);// 银商订单号 最大长度26位
                        resJSON.TryGetString("thirdPartyBuyerId", out string thirdPartyBuyerId); // 第三方买家Id 最大长度32位
                        resJSON.TryGetString("thirdPartyBuyerId", out string thirdPartyOrderId);// 第三方订单号
                        resJSON.TryGetString("orderStatus", out string orderStatus);// 订单状态 TRADE_CLOSED、TRADE_SUCCESS、TRADE_REFUND、WAIT_BUYER_PAY、NEW_ORDER、UNKNOWN
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        channelRetMsg.ChannelOrderId = orderId;
                        channelRetMsg.ChannelUserId = thirdPartyBuyerId;
                        channelRetMsg.PlatformOrderId = thirdPartyOrderId;
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
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }

        private ChannelRetMsg QrQuery(string logPrefix, ChannelRetMsg channelRetMsg, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));// 报文请求时间 格式：yyyy-MM-dd HH:mm:ss
            reqParams.Add("instMid", "QRPAYDEFAULT");// 业务类型 QRPAYDEFAULT
            reqParams.Add("billNo", payOrder.PayOrderId); // 账单号
            reqParams.Add("billDate", payOrder.CreatedAt?.ToString("yyyy-MM-dd")); // 订单时间 格式：yyyy-MM-dd
            // 封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = umsPayPaymentService.PackageParamAndReq("/v1/netpay/bills/query", reqParams, logPrefix, mchAppConfigContext);

            // 请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            resJSON.TryGetString("errInfo", out string errInfo); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("billStatus", out string billStatus);// 账单状态
                        switch (billStatus)
                        {
                            case "PAID":
                                resJSON.TryGetValue("billPayment", out JToken billPayment); // 账单支付信息
                                ((JObject)billPayment).TryGetString("paySeqId", out string paySeqId);// 交易参考号
                                ((JObject)billPayment).TryGetString("buyerId", out string buyerId);// 交易参考号
                                ((JObject)billPayment).TryGetString("targetOrderId", out string targetOrderId);// 目标平台单号
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                channelRetMsg.ChannelOrderId = paySeqId;
                                channelRetMsg.ChannelUserId = buyerId;
                                channelRetMsg.PlatformOrderId = targetOrderId;
                                break;
                            case "UNPAID":
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                break;
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
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }

        private ChannelRetMsg UnifiedQuery(string logPrefix, ChannelRetMsg channelRetMsg, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            reqParams.Add("requestTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //reqParams.Add("srcReserve", "webpay");
            reqParams.Add("instMid", "YUEDANDEFAULT");
            reqParams.Add("merOrderId", payOrder.PayOrderId); // 商户订单号
                                                              //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
            JObject resJSON = umsPayPaymentService.PackageParamAndReq("/v1/netpay/query", reqParams, logPrefix, mchAppConfigContext);

            //请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            resJSON.TryGetString("errInfo", out string errInfo); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "SUCCESS":
                        resJSON.TryGetString("seqId", out string seqId);// 平台流水号
                        resJSON.TryGetString("settleRefId", out string settleRefId);// 清分ID 如果来源方传了bankRefId就等于bankRefId，否则等于seqId
                        resJSON.TryGetString("buyerId", out string buyerId); // 买家ID
                        resJSON.TryGetString("targetOrderId", out string targetOrderId);// 目标平台单号
                        resJSON.TryGetString("status", out string status);// 订单状态 TRADE_CLOSED、TRADE_SUCCESS、TRADE_REFUND、WAIT_BUYER_PAY、NEW_ORDER、UNKNOWN
                        resJSON.TryGetString("goodsTradeNo", out string goodsTradeNo);// 商品交易单号
                        switch (status)
                        {
                            case "TRADE_SUCCESS":
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                break;
                            case "WAIT_BUYER_PAY":
                            default:
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                break;
                        }
                        channelRetMsg.ChannelOrderId = seqId;
                        channelRetMsg.ChannelUserId = buyerId;
                        channelRetMsg.PlatformOrderId = targetOrderId;
                        channelRetMsg.PlatformMchOrderId = goodsTradeNo;
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
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            return channelRetMsg;
        }
    }
}
