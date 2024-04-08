using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.LklPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.LklPay
{
    /// <summary>
    /// 拉卡拉退款接口
    /// </summary>
    public class LklPayRefundService : AbstractRefundService
    {
        private readonly ILogger<LklPayRefundService> _logger;
        private readonly LklPayPaymentService lklpayPaymentService;
        public LklPayRefundService(ILogger<LklPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            _logger = logger;
            this.lklpayPaymentService = ActivatorUtilities.CreateInstance<LklPayPaymentService>(serviceProvider);
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LKLPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string transType = LklPayEnum.GetTransType(refundOrder.WayCode);
            string logPrefix = $"【拉卡拉({transType})退款查询】";
            try
            {
                reqParams.Add("out_refund_order_no", refundOrder.RefundOrderId); // 退款订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = lklpayPaymentService.PackageParamAndReq("/api/v3/labs/query/idmrefundquery", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON?.GetValue("code").ToString(); //业务响应码
                string msg = resJSON?.GetValue("msg").ToString(); //业务响应信息	
                if ("BBS00000".Equals(code))
                {
                    var respData = resJSON.GetValue("req_data").ToObject<JObject>();
                    string tradeState = respData.GetValue("trade_state").ToString();
                    string tradeNo = respData.GetValue("trade_no").ToString();//拉卡拉商户订单号
                    string accTradeNo = respData.GetValue("acc_trade_no").ToString();//拉卡拉商户订单号
                    var orderStatus = LklPayEnum.ConvertTradeState(tradeState);
                    switch (orderStatus)
                    {
                        case LklPayEnum.TradeState.SUCCESS:
                            channelRetMsg.ChannelOrderId = tradeNo;
                            channelRetMsg.PlatformOrderId = accTradeNo;
                            channelRetMsg.PlatformMchOrderId = tradeNo;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            _logger.LogInformation($"{logPrefix} >>> 退款成功");
                            break;
                        case LklPayEnum.TradeState.FAIL:
                            //明确退款失败
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = code;
                            channelRetMsg.ChannelErrMsg = msg;
                            _logger.LogInformation($"{logPrefix} >>> 退款失败, {msg}");
                            break;
                        case LklPayEnum.TradeState.DEAL:
                            //退款中
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            _logger.LogInformation($"{logPrefix} >>> 退款中");
                            break;
                    }
                }
                else if ("BBS11112".Equals(code) || "BBS11105".Equals(code) || "BBS10000".Equals(code))
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
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

        public override ChannelRetMsg Refund(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string transType = LklPayEnum.GetTransType(refundOrder.WayCode);
            string logPrefix = $"【拉卡拉({transType})订单退款】";
            try
            {
                reqParams.Add("out_refund_order_no", refundOrder.RefundOrderId); // 商户退款订单号
                reqParams.Add("origin_out_trade_no", payOrder.PayOrderId); // 原商户交易流水号
                reqParams.Add("refund_amount", refundOrder.RefundAmount); // 退款金额
                //reqParams.Add("notify_url", GetNotifyUrl());
                reqParams.Add("refund_reason", refundOrder.RefundReason); // 退款原因

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = lklpayPaymentService.PackageParamAndReq("/api/v3/labs/relation/idmrefund", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON?.GetValue("code").ToString(); //业务响应码
                string msg = resJSON?.GetValue("msg").ToString(); //业务响应信息	
                if ("BBS00000".Equals(code))
                {
                    var respData = resJSON.GetValue("req_data").ToObject<JObject>();
                    string tradeNo = respData.GetValue("trade_no").ToString();//拉卡拉商户订单号
                    string accTradeNo = respData.GetValue("acc_trade_no").ToString();//拉卡拉商户订单号
                    channelRetMsg.ChannelOrderId = tradeNo;
                    channelRetMsg.PlatformOrderId = accTradeNo;
                    channelRetMsg.PlatformMchOrderId = tradeNo;
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    _logger.LogInformation($"{logPrefix} >>> 退款成功");
                }
                else if ("BBS11112".Equals(code) || "BBS11105".Equals(code) || "BBS10000".Equals(code))
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    _logger.LogInformation($"{logPrefix} >>> 退款中");
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
                    _logger.LogInformation($"{logPrefix} >>> 退款失败, {msg}");
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
