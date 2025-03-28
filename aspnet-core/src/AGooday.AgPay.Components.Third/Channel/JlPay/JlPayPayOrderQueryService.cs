﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.JlPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.JlPay
{
    /// <summary>
    /// 嘉联查单
    /// </summary>
    public class JlPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<JlPayPayOrderQueryService> _logger;
        private readonly JlPayPaymentService _paymentService;

        public JlPayPayOrderQueryService(ILogger<JlPayPayOrderQueryService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _paymentService = ActivatorUtilities.CreateInstance<JlPayPaymentService>(serviceProvider);
        }

        public JlPayPayOrderQueryService()
        {
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.JLPAY;
        }

        public async Task<ChannelRetMsg> QueryAsync(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string payType = JlPayEnum.GetPayType(payOrder.WayCode);
            string logPrefix = $"【嘉联({payType})查单】";

            try
            {
                reqParams.Add("out_trade_no", payOrder.PayOrderId); //订单号

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/api/pay/chnquery", reqParams, logPrefix, mchAppConfigContext, false);
                _logger.LogInformation("查询订单 payorderId={PayOrderId}, 返回结果: {resData}", payOrder.PayOrderId, JsonConvert.SerializeObject(resJSON));
                //_logger.LogInformation($"查询订单 payorderId={payOrder.PayOrderId}, 返回结果: {JsonConvert.SerializeObject(resJSON)}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                var channelRetMsg = ChannelRetMsg.Waiting();
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
                    resJSON.TryGetString("chn_transaction_id", out string chnTransactionId);//用户账单上的交易订单号	
                    resJSON.TryGetString("sub_openid", out string subOpenid);
                    string _status = resJSON.GetValue("status").ToString();
                    var status = JlPayEnum.ConvertStatus(_status);
                    switch (status)
                    {
                        case JlPayEnum.Status.Success:
                            channelRetMsg = ChannelRetMsg.ConfirmSuccess(transactionId);  //支付成功
                            channelRetMsg.ChannelMchNo = mchId;
                            channelRetMsg.ChannelOrderId = transactionId;
                            channelRetMsg.ChannelUserId = subOpenid;
                            channelRetMsg.PlatformOrderId = chnTransactionId;
                            channelRetMsg.PlatformMchOrderId = transactionId;
                            break;
                        case JlPayEnum.Status.Failure:
                            channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                            channelRetMsg.ChannelErrCode = retCode;
                            channelRetMsg.ChannelErrMsg = retMsg;
                            break;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = retCode;
                    channelRetMsg.ChannelErrMsg = retMsg;
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
