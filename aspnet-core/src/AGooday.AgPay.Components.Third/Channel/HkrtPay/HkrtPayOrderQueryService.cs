using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.HkrtPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.HkrtPay
{
    /// <summary>
    /// 海科融通查单
    /// </summary>
    public class HkrtPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<HkrtPayPayOrderQueryService> _logger;
        private readonly HkrtPayPaymentService _paymentService;

        public HkrtPayPayOrderQueryService(ILogger<HkrtPayPayOrderQueryService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _paymentService = ActivatorUtilities.CreateInstance<HkrtPayPaymentService>(serviceProvider);
        }

        public HkrtPayPayOrderQueryService()
        {
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.HKRTPAY;
        }

        public async Task<ChannelRetMsg> QueryAsync(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string payType = HkrtPayEnum.GetTradeType(payOrder.WayCode);
            string logPrefix = $"【海科融通({payType})查单】";

            try
            {
                reqParams.Add("out_trade_no", payOrder.PayOrderId); //服务商交易订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/api/v1/pay/polymeric/query", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                var channelRetMsg = ChannelRetMsg.Waiting();
                //请求 & 响应成功， 判断业务逻辑
                string return_code = resJSON.GetValue("return_code").ToString(); //返回状态码
                resJSON.TryGetString("return_msg", out string return_msg); //返回错误信息
                if ("10000".Equals(return_code))
                {
                    resJSON.TryGetString("error_code", out string error_code); //错误码
                    resJSON.TryGetString("error_msg", out string error_msg); //错误码描述
                    if (!string.IsNullOrWhiteSpace(error_code))
                    {
                        string status = resJSON.GetValue("trade_status").ToString();
                        string type = resJSON.GetValue("trade_type").ToString();
                        string trade_no = resJSON.GetValue("trade_no").ToString();//交易订单号 SaaS平台的交易订单编号
                        string channel_trade_no = resJSON.GetValue("channel_trade_no").ToString();//凭证条码订单号
                        resJSON.TryGetString("alipay_no", out string alipay_no);
                        resJSON.TryGetString("weixin_no", out string weixin_no);
                        var tradeStatus = HkrtPayEnum.ConvertTradeStatus(status);
                        switch (tradeStatus)
                        {
                            case HkrtPayEnum.TradeStatus.Success:
                                channelRetMsg = ChannelRetMsg.ConfirmSuccess(trade_no);  //支付成功
                                channelRetMsg.ChannelOrderId = trade_no;
                                var tradeType = HkrtPayEnum.ConvertTradeType(type);
                                var attach = _paymentService.GetAttach(resJSON);
                                attach.TryGetString("out_trade_no", out string out_trade_no);
                                channelRetMsg.PlatformMchOrderId = out_trade_no;
                                switch (tradeType)
                                {
                                    case HkrtPayEnum.TradeType.WX:
                                        attach.TryGetString("sub_openid", out string sub_openid);
                                        attach.TryGetString("transaction_id", out string transaction_id);
                                        channelRetMsg.ChannelUserId = sub_openid;
                                        channelRetMsg.PlatformOrderId = transaction_id;
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
                                break;
                            case HkrtPayEnum.TradeStatus.Failed:
                                channelRetMsg = ChannelRetMsg.ConfirmFail(error_code, error_msg);
                                break;
                            case HkrtPayEnum.TradeStatus.Paying: // 支付中
                            case HkrtPayEnum.TradeStatus.Timeout:
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
                return channelRetMsg; //支付中
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"查询订单 payorderId:{payOrder.PayOrderId}, 异常:{e.Message}");
                return ChannelRetMsg.Waiting(); //支付中
            }
        }
    }
}
