﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using WechatTenpayClientV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClient;
using WechatTenpayClientV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClient;

namespace AGooday.AgPay.Components.Third.Channel.WxPay
{
    /// <summary>
    /// 微信查单接口
    /// </summary>
    public class WxPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<WxPayPayOrderQueryService> _logger;
        private readonly ConfigContextQueryService _configContextQueryService;

        public WxPayPayOrderQueryService(ILogger<WxPayPayOrderQueryService> logger, ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _configContextQueryService = configContextQueryService;
        }

        public WxPayPayOrderQueryService()
        {
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public async Task<ChannelRetMsg> QueryAsync(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                WxServiceWrapper wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);

                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion)) // V2
                {
                    GetPayOrderRequest request = new GetPayOrderRequest();

                    //放置isv信息
                    //不是特约商户，无需放置此值
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        WxPayIsvSubMchParams isvsubMchParams =
                            (WxPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.SubAppId = isvsubMchParams.SubMchAppId;
                    }

                    request.OutTradeNumber = payOrder.PayOrderId;

                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    var result = await client.ExecuteGetPayOrderAsync(request);

                    string channelState = result.TradeState;
                    string transactionId = result.TransactionId;
                    if ("SUCCESS".Equals(channelState)) //支付成功
                    {
                        return ChannelRetMsg.ConfirmSuccess(transactionId);
                    }
                    else if ("USERPAYING".Equals(channelState)) //支付中，等待用户输入密码
                    {
                        return ChannelRetMsg.Waiting(); //支付中
                    }
                    else if ("CLOSED".Equals(channelState)
                        || "REVOKED".Equals(channelState)
                        || "PAYERROR".Equals(channelState)) //CLOSED—已关闭， REVOKED—已撤销(刷卡支付), PAYERROR--支付失败(其他原因，如银行返回失败)
                    {
                        return ChannelRetMsg.ConfirmFail(); //支付失败
                    }
                    else
                    {
                        return ChannelRetMsg.Unknown();
                    }
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion)) // V3
                {
                    string channelState = string.Empty;
                    string transactionId = string.Empty;
                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    if (mchAppConfigContext.IsIsvSubMch()) // 特约商户
                    {
                        WxPayIsvSubMchParams isvsubMchParams = (WxPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                        GetPayPartnerTransactionByOutTradeNumberRequest request = new GetPayPartnerTransactionByOutTradeNumberRequest();
                        request.MerchantId = wxServiceWrapper.Config.MchId;
                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.OutTradeNumber = payOrder.PayOrderId;
                        GetPayPartnerTransactionByOutTradeNumberResponse result = await client.ExecuteGetPayPartnerTransactionByOutTradeNumberAsync(request);
                        channelState = result.TradeState;
                        transactionId = result.TransactionId;
                    }
                    else
                    {
                        GetPayTransactionByOutTradeNumberRequest request = new GetPayTransactionByOutTradeNumberRequest();
                        request.MerchantId = wxServiceWrapper.Config.MchId;
                        request.OutTradeNumber = payOrder.PayOrderId;
                        GetPayTransactionByOutTradeNumberResponse result = await client.ExecuteGetPayTransactionByOutTradeNumberAsync(request);
                        channelState = result.TradeState;
                        transactionId = result.TransactionId;
                    }

                    if ("SUCCESS".Equals(channelState))
                    {
                        return ChannelRetMsg.ConfirmSuccess(transactionId);
                    }
                    else if ("USERPAYING".Equals(channelState)) //支付中，等待用户输入密码
                    {
                        return ChannelRetMsg.Waiting(); //支付中
                    }
                    else if ("CLOSED".Equals(channelState) ||
                             "REVOKED".Equals(channelState) ||
                             "PAYERROR".Equals(channelState)) //CLOSED—已关闭， REVOKED—已撤销(刷卡支付), PAYERROR--支付失败(其他原因，如银行返回失败)
                    {
                        return ChannelRetMsg.ConfirmFail(); //支付失败
                    }
                    else
                    {
                        return ChannelRetMsg.Unknown();
                    }
                }
                return ChannelRetMsg.ConfirmFail();
            }
            catch (Exception e)
            {
                return ChannelRetMsg.SysError(e.Message);
            }
        }
    }
}
