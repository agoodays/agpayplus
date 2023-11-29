using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.LesPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.LesPay
{
    /// <summary>
    /// 乐刷退款接口
    /// </summary>
    public class LesPayRefundService : AbstractRefundService
    {
        private readonly ILogger<LesPayRefundService> log;
        private readonly LesPayPaymentService lesPayPaymentService;
        public LesPayRefundService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService,
            ILogger<LesPayRefundService> log,
            LesPayPaymentService lesPayPaymentService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            this.log = log;
            this.lesPayPaymentService = lesPayPaymentService;
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.LESPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            string payType = LesPayEnum.GetPayWay(refundOrder.WayCode);
            string logPrefix = $"【乐刷({payType})退款查询】";
            try
            {
                reqParams.Add("service", "unified_query_refund"); //订单号
                reqParams.Add("third_order_id", refundOrder.PayOrderId); // 原交易订单号
                reqParams.Add("merchant_refund_id", refundOrder.RefundOrderId); // 退款订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = lesPayPaymentService.PackageParamAndReq("/cgi-bin/lepos_pay_gateway.cgi", reqParams, logPrefix, mchAppConfigContext);
                log.LogInformation($"查询订单 refundOrderId:{refundOrder.RefundOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string resp_code = resJSON.GetValue("resp_code").ToString(); //返回状态码
                resJSON.TryGetString("resp_msg", out string resp_msg); //返回错误信息
                if ("0".Equals(resp_code))
                {
                    string result_code = resJSON.GetValue("result_code").ToString(); //业务结果
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if ("0".Equals(result_code))
                    {
                        string status = resJSON.GetValue("status").ToString();
                        string leshua_refund_id = resJSON.GetValue("leshua_refund_id").ToString();//乐刷退款id
                        resJSON.TryGetString("sub_merchant_id", out string sub_merchant_id);//渠道商商户号
                        var orderStatus = LesPayEnum.ConvertOrderStatus(status);
                        switch (orderStatus)
                        {
                            case LesPayEnum.OrderStatus.RefundSuccess:
                                channelRetMsg.ChannelOrderId = leshua_refund_id;
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                log.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case LesPayEnum.OrderStatus.RefundFail:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = error_code;
                                channelRetMsg.ChannelErrMsg = error_msg;
                                log.LogInformation($"{logPrefix} >>> 退款失败, {error_msg}");
                                break;
                            case LesPayEnum.OrderStatus.Refunding:
                                //退款中
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                log.LogInformation($"{logPrefix} >>> 退款中");
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
                    channelRetMsg.ChannelErrCode = resp_code;
                    channelRetMsg.ChannelErrMsg = resp_msg;
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
            SortedDictionary<string, string> reqParams = new SortedDictionary<string, string>();
            string payType = LesPayEnum.GetPayWay(refundOrder.WayCode);
            string logPrefix = $"【乐刷({payType})订单退款】";
            try
            {
                reqParams.Add("service", "unified_refund"); //订单号
                reqParams.Add("merchant_refund_id", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("third_order_id", payOrder.PayOrderId); // 原交易订单号
                reqParams.Add("refund_amount", refundOrder.RefundAmount.ToString()); // 退款金额
                reqParams.Add("notify_url", GetNotifyUrl()); // 订单类型

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = lesPayPaymentService.PackageParamAndReq("/cgi-bin/lepos_pay_gateway.cgi", reqParams, logPrefix, mchAppConfigContext);
                log.LogInformation($"订单退款 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }
                //请求 & 响应成功， 判断业务逻辑
                string resp_code = resJSON.GetValue("resp_code").ToString(); //返回状态码
                resJSON.TryGetString("resp_msg", out string resp_msg); //返回错误信息
                if ("0".Equals(resp_code))
                {
                    string result_code = resJSON.GetValue("result_code").ToString(); //业务结果
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if ("0".Equals(result_code))
                    {
                        string status = resJSON.GetValue("status").ToString();
                        string leshua_refund_id = resJSON.GetValue("leshua_refund_id").ToString();//乐刷退款id
                        resJSON.TryGetString("sub_merchant_id", out string sub_merchant_id);//渠道商商户号
                        var orderStatus = LesPayEnum.ConvertOrderStatus(status);
                        switch (orderStatus)
                        {
                            case LesPayEnum.OrderStatus.RefundSuccess:
                                channelRetMsg.ChannelOrderId = leshua_refund_id;
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                log.LogInformation($"{logPrefix} >>> 退款成功");
                                break;
                            case LesPayEnum.OrderStatus.RefundFail:
                                //明确退款失败
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                channelRetMsg.ChannelErrCode = error_code;
                                channelRetMsg.ChannelErrMsg = error_msg;
                                log.LogInformation($"{logPrefix} >>> 退款失败, {error_msg}");
                                break;
                            case LesPayEnum.OrderStatus.Refunding:
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
