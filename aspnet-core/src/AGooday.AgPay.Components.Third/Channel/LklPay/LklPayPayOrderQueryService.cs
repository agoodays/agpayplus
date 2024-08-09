using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.LklPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.LklPay
{
    /// <summary>
    /// 拉卡拉查单
    /// </summary>
    public class LklPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<LklPayPayOrderQueryService> _logger;
        private readonly LklPayPaymentService lklpayPaymentService;

        public LklPayPayOrderQueryService(ILogger<LklPayPayOrderQueryService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            this.lklpayPaymentService = ActivatorUtilities.CreateInstance<LklPayPaymentService>(serviceProvider);
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.LKLPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string transType = LklPayEnum.GetTransType(payOrder.WayCode);
            string logPrefix = $"【拉卡拉({transType})查单】";

            try
            {
                reqParams.Add("out_trade_no", payOrder.PayOrderId); //订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = lklpayPaymentService.PackageParamAndReq("/api/v3/labs/query/tradequery", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation($"查询订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                var channelRetMsg = ChannelRetMsg.Waiting();
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON?.GetValue("code").ToString(); //业务响应码
                string msg = resJSON?.GetValue("msg").ToString(); //业务响应信息	
                if ("BBS00000".Equals(code))
                {
                    var respData = resJSON.GetValue("req_data").ToObject<JObject>();
                    respData.TryGetString("merchant_no", out string merchantNo);
                    string tradeNo = respData.GetValue("trade_no").ToString();//拉卡拉商户订单号
                    string accTradeNo = respData.GetValue("acc_trade_no").ToString();//拉卡拉商户订单号
                    respData.TryGetString("user_id1", out string userId1);
                    respData.TryGetString("user_id2", out string userId2);
                    string tradeState = respData.GetValue("trade_state").ToString();
                    var orderStatus = LklPayEnum.ConvertTradeState(tradeState);
                    switch (orderStatus)
                    {
                        case LklPayEnum.TradeState.SUCCESS:
                            channelRetMsg = ChannelRetMsg.ConfirmSuccess(tradeNo);  //支付成功
                            channelRetMsg.ChannelMchNo = merchantNo;
                            channelRetMsg.ChannelUserId = userId2 ?? userId1;
                            channelRetMsg.PlatformOrderId = accTradeNo;
                            channelRetMsg.PlatformMchOrderId = tradeNo;
                            break;
                        case LklPayEnum.TradeState.FAIL:
                            channelRetMsg = ChannelRetMsg.ConfirmFail(code, msg);
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
