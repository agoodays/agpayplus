using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.JlPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.JlPay
{
    /// <summary>
    /// 嘉联退款接口
    /// </summary>
    public class JlPayRefundService : AbstractRefundService
    {
        private readonly JlPayPaymentService _paymentService;

        public JlPayRefundService(ILogger<JlPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
            _paymentService = ActivatorUtilities.CreateInstance<JlPayPaymentService>(serviceProvider);
        }

        public JlPayRefundService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.JLPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        public override async Task<ChannelRetMsg> QueryAsync(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            JObject reqParams = new JObject();
            string payType = JlPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【嘉联({payType})退款查询】";
            try
            {
                reqParams.Add("out_trade_no", refundOrder.PayOrderId); //退款订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/api/pay/chnquery", reqParams, logPrefix, mchAppConfigContext, false);
                _logger.LogInformation("查询订单 refundOrderId={PayOrderId}, 返回结果: {resJSON}", refundOrder.PayOrderId, JsonConvert.SerializeObject(resJSON));
                //_logger.LogInformation($"查询订单 refundOrderId={refundOrder.PayOrderId}, 返回结果: {JsonConvert.SerializeObject(resJSON)}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                //请求 & 响应成功， 判断业务逻辑
                string retCode = resJSON?.GetValue("ret_code").ToString(); //业务响应码
                string retMsg = resJSON?.GetValue("ret_msg").ToString(); //业务响应信息	
                string mchId = resJSON?.GetValue("mch_id")?.ToString();
                string orgCode = resJSON?.GetValue("org_code")?.ToString();
                channelRetMsg.ChannelMchNo = mchId;
                channelRetMsg.ChannelIsvNo = orgCode;
                if ("00".Equals(retCode))
                {
                    resJSON.TryGetString("transaction_id", out string transactionId);
                    resJSON.TryGetString("sub_openid", out string subOpenid);
                    string _status = resJSON.GetValue("status").ToString();
                    var status = JlPayEnum.ConvertStatus(_status);
                    switch (status)
                    {
                        case JlPayEnum.Status.Success:
                            //退款成功
                            channelRetMsg.ChannelOrderId = transactionId;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            _logger.LogInformation("{logPrefix} >>> 退款成功", logPrefix);
                            //_logger.LogInformation($"{logPrefix} >>> 退款成功");
                            break;
                        case JlPayEnum.Status.Failure:
                            //明确退款失败
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = retCode;
                            channelRetMsg.ChannelErrMsg = retMsg;
                            _logger.LogInformation("{logPrefix} >>> 退款失败, {retMsg}", logPrefix, retMsg);
                            //_logger.LogInformation($"{logPrefix} >>> 退款失败, {retMsg}");
                            break;
                        case JlPayEnum.Status.Pending:
                            //退款中
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            _logger.LogInformation("{logPrefix} >>> 退款中", logPrefix);
                            //_logger.LogInformation($"{logPrefix} >>> 退款中");
                            break;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = retCode;
                    channelRetMsg.ChannelErrMsg = retMsg;
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
            string payType = JlPayEnum.GetPayType(refundOrder.WayCode);
            string logPrefix = $"【嘉联({payType})订单退款】";
            try
            {
                reqParams.Add("out_trade_no", refundOrder.RefundOrderId); // 退款订单号
                reqParams.Add("ori_out_trade_no", payOrder.PayOrderId); // 原交易订单号
                reqParams.Add("total_fee", refundOrder.RefundAmount); // 退款金额
                reqParams.Add("remark", refundOrder.RefundReason); // 退货原因

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/api/pay/refund", reqParams, logPrefix, mchAppConfigContext, false);
                _logger.LogInformation("订单退款 payorderId={PayOrderId}, 返回结果: {resData}", payOrder.PayOrderId, JsonConvert.SerializeObject(resJSON));
                //_logger.LogInformation($"订单退款 payorderId={payOrder.PayOrderId}, 返回结果: {JsonConvert.SerializeObject(resJSON)}");
                if (resJSON == null)
                {
                    channelRetMsg.ChannelState = ChannelState.UNKNOWN; // 状态不明确
                }

                //请求 & 响应成功， 判断业务逻辑
                string retCode = resJSON?.GetValue("ret_code").ToString(); //业务响应码
                string retMsg = resJSON?.GetValue("ret_msg").ToString(); //业务响应信息	
                string mchId = resJSON?.GetValue("mch_id")?.ToString();
                string orgCode = resJSON?.GetValue("org_code")?.ToString();
                channelRetMsg.ChannelMchNo = mchId;
                channelRetMsg.ChannelIsvNo = orgCode;
                if ("00".Equals(retCode))
                {
                    resJSON.TryGetString("transaction_id", out string transactionId);
                    resJSON.TryGetString("sub_openid", out string subOpenid);
                    string _status = resJSON.GetValue("status").ToString();
                    var status = JlPayEnum.ConvertStatus(_status);
                    switch (status)
                    {
                        case JlPayEnum.Status.Success:
                            //退款成功
                            channelRetMsg.ChannelOrderId = transactionId;
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            _logger.LogInformation("{logPrefix} >>> 退款成功", logPrefix);
                            //_logger.LogInformation($"{logPrefix} >>> 退款成功");
                            break;
                        case JlPayEnum.Status.Failure:
                            //明确退款失败
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = retCode;
                            channelRetMsg.ChannelErrMsg = retMsg;
                            _logger.LogInformation("{logPrefix} >>> 退款失败, {retMsg}", logPrefix, retMsg);
                            //_logger.LogInformation($"{logPrefix} >>> 退款失败, {retMsg}");
                            break;
                        case JlPayEnum.Status.Pending:
                            //退款中
                            channelRetMsg.ChannelOrderId = transactionId;
                            channelRetMsg.ChannelState = ChannelState.WAITING;
                            _logger.LogInformation("{logPrefix} >>> 退款中", logPrefix);
                            //_logger.LogInformation($"{logPrefix} >>> 退款中");
                            break;
                    }
                }
                else
                {
                    channelRetMsg.ChannelErrCode = retCode;
                    channelRetMsg.ChannelErrMsg = retMsg;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{logPrefix}, 异常: {Message}", logPrefix, e.Message);
                //_logger.LogError(e, $"{logPrefix}, 异常: {e.Message}");
                channelRetMsg.ChannelState = ChannelState.SYS_ERROR; // 系统异常
            }
            return channelRetMsg;
        }
    }
}
