using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.SxfPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.SxfPay
{
    /// <summary>
    /// 随行付退款接口
    /// </summary>
    public class SxfPayRefundService : AbstractRefundService
    {
        private readonly SxfPayPaymentService _paymentService;
        public SxfPayRefundService(ILogger<SxfPayRefundService> logger,
            //[FromKeyedServices(CS.IF_CODE.SXFPAY)] IPaymentService paymentService,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
            //_paymentService = (SxfPayPaymentService)paymentService;
            //_paymentService = (SxfPayPaymentService)serviceProvider.GetRequiredKeyedService<IPaymentService>(CS.IF_CODE.SXFPAY);
            _paymentService = ActivatorUtilities.CreateInstance<SxfPayPaymentService>(serviceProvider);
        }

        public SxfPayRefundService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.SXFPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override long CalculateFeeAmount(long amount, PayOrderDto payOrder)
        {
            var refundState = payOrder.RefundAmount + amount >= payOrder.Amount ? PayOrderRefund.REFUND_STATE_ALL : PayOrderRefund.REFUND_STATE_SUB;
            if (refundState.Equals(PayOrderRefund.REFUND_STATE_ALL))
            {
                return payOrder.MchFeeAmount;
            }
            /**
             * 退款手续费规则说明：https://paas.tianquetech.com/docs/#/help/jsdj 
             * 退款手续费计算公式如下：
             * 全额退款的手续费返还：退款手续费=原交易手续费
             * 部分退款的手续费返还：部分退款金额/原交易金额*原交易手续费 【手续费逐笔计算，四舍五入保留两位小数】。
             **/
            return AmountUtil.CalPercentageFee(amount, payOrder.MchOrderFeeAmount, payOrder.Amount);
        }

        public override async Task<ChannelRetMsg> QueryAsync(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = SxfPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【随行付({payType})退款查询】";
            try
            {
                reqParams.Add("ordNo", refundOrder.RefundOrderId); // 退款订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/query/refundQuery", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
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
                                _logger.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case SxfPayEnum.OrderStatus.REFUNDFAIL:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = bizCode;
                                channelRetMsg.ChannelErrMsg = bizMsg;
                                _logger.LogInformation($"{logPrefix} >>> 退款失败, {bizMsg}");
                                break;
                            case SxfPayEnum.OrderStatus.REFUNDING:
                                //退款中
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                _logger.LogInformation($"{logPrefix} >>> 退款中");
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
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/order/refund", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
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
                    if ("0000".Equals(bizCode) || "0002".Equals(bizCode))
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
                                _logger.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case SxfPayEnum.OrderStatus.REFUNDFAIL:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = bizCode;
                                channelRetMsg.ChannelErrMsg = bizMsg;
                                _logger.LogInformation($"{logPrefix} >>> 退款失败, {bizMsg}");
                                break;
                            case SxfPayEnum.OrderStatus.REFUNDING:
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
