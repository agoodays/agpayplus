using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.LcswPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.LcswPay
{
    /// <summary>
    /// 利楚扫呗查单
    /// </summary>
    public class LcswPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<LcswPayPayOrderQueryService> _logger;
        private readonly LcswPayPaymentService _paymentService;

        public LcswPayPayOrderQueryService(ILogger<LcswPayPayOrderQueryService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _paymentService = ActivatorUtilities.CreateInstance<LcswPayPaymentService>(serviceProvider);
        }

        public LcswPayPayOrderQueryService()
        {
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.LCSWPAY;
        }

        public async Task<ChannelRetMsg> QueryAsync(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string payType = LcswPayEnum.GetPayType(payOrder.WayCode);
            string logPrefix = $"【利楚扫呗({payType})查单】";

            try
            {
                reqParams.Add("pay_ver", "202");
                reqParams.Add("pay_type", payType);
                reqParams.Add("service_id", "020");
                reqParams.Add("terminal_trace", Guid.NewGuid().ToString("N"));
                reqParams.Add("terminal_time", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/pay/open/query", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation("查询订单 payorderId={payOrder.PayOrderId}, 返回结果: {resData}", payOrder.PayOrderId, JsonConvert.SerializeObject(resJSON));
                //_logger.LogInformation($"查询订单 payorderId={payOrder.PayOrderId}, 返回结果: {JsonConvert.SerializeObject(resJSON)}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                var channelRetMsg = ChannelRetMsg.Waiting();
                //请求 & 响应成功， 判断业务逻辑
                string returnCode = resJSON.GetValue("return_code").ToString(); //请求响应码
                string returnMsg = resJSON.GetValue("return_msg").ToString(); //响应信息
                resJSON.TryGetString("merchant_no", out string merchantNo); // 商户号
                channelRetMsg.ChannelMchNo = merchantNo;
                if ("01".Equals(returnCode))
                {
                    resJSON.TryGetString("result_code", out string resultCode); // 业务结果
                    if ("01".Equals(resultCode) || "02".Equals(resultCode) || "03".Equals(resultCode))
                    {
                        if ("01".Equals(resultCode) || "02".Equals(resultCode))
                        {
                            resJSON.TryGetString("out_trade_no", out string outTradeNo);// 平台唯一订单号
                            resJSON.TryGetString("channel_trade_no", out string channelTradeNo);// 微信/支付宝流水号
                            resJSON.TryGetString("channel_order_no", out string channelOrderNo);// 银行渠道订单号，微信支付时显示在支付成功页面的条码，可用作扫码查询和扫码退款时匹配
                            resJSON.TryGetString("user_id", out string userId);// 付款方用户id，服务商appid下的“微信openid”、“支付宝账户”
                            resJSON.TryGetString("trade_state", out string tradeState);
                            var _tradeState = LcswPayEnum.ConvertTradeState(tradeState);
                            switch (_tradeState)
                            {
                                case LcswPayEnum.TradeState.SUCCESS:
                                case LcswPayEnum.TradeState.REFUND:
                                    channelRetMsg = ChannelRetMsg.ConfirmSuccess(outTradeNo);  //支付成功
                                    channelRetMsg.ChannelMchNo = merchantNo;
                                    channelRetMsg.ChannelOrderId = outTradeNo;
                                    channelRetMsg.ChannelUserId = userId;
                                    channelRetMsg.PlatformOrderId = channelTradeNo;
                                    channelRetMsg.PlatformMchOrderId = channelOrderNo;
                                    break;
                                case LcswPayEnum.TradeState.CLOSED:
                                case LcswPayEnum.TradeState.REVOKED:
                                case LcswPayEnum.TradeState.NOPAY:
                                case LcswPayEnum.TradeState.PAYERROR:
                                    channelRetMsg = ChannelRetMsg.ConfirmFail(resultCode, returnMsg);
                                    break;
                                case LcswPayEnum.TradeState.NOTPAY:
                                case LcswPayEnum.TradeState.USERPAYING:
                                    break;
                            }
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
                return channelRetMsg; //支付中
            }
            catch (Exception e)
            {
                _logger.LogError(e, "查询订单 payorderId={PayOrderId}, 异常: {Message}", payOrder.PayOrderId, e.Message);
                //_logger.LogError(e, $"查询订单 payorderId={payOrder.PayOrderId}, 异常: {e.Message}");
                return ChannelRetMsg.Waiting(); //支付中
            }
        }
    }
}
