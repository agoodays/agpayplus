using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Channel.WxPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
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
    /// 退款接口： 微信官方
    /// </summary>
    public class WxPayRefundService : AbstractRefundService
    {
        public WxPayRefundService(ILogger<WxPayRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public WxPayRefundService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public override string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder)
        {
            return null;
        }

        /// <summary>
        /// 微信退款查单接口
        /// </summary>
        /// <param name="refundOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<ChannelRetMsg> QueryAsync(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                ChannelRetMsg channelRetMsg = new ChannelRetMsg();

                WxServiceWrapper wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);

                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion))  // V2
                {
                    GetPayRefundV2Request request = new GetPayRefundV2Request();

                    //放置isv信息
                    //不是特约商户，无需放置此值
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        WxPayIsvSubMchParams isvsubMchParams =
                            (WxPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.SubAppId = isvsubMchParams.SubMchAppId;
                    }

                    request.OutRefundNumber = refundOrder.RefundOrderId; // 退款单号
                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    var result = await client.ExecuteGetPayRefundV2Async(request);

                    if ("SUCCESS".Equals(result.ResultCode))  // 退款成功
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelErrMsg = result.ReturnMessage;
                    }
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion))  // V3
                {
                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    GetRefundDomesticRefundByOutRefundNumberRequest request = new GetRefundDomesticRefundByOutRefundNumberRequest();
                    request.OutRefundNumber = refundOrder.RefundOrderId;
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        WxPayIsvSubMchParams isvsubMchParams = (WxPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                        request.SubMerchantId = isvsubMchParams.SubMchId;
                    }
                    var result = await client.ExecuteGetRefundDomesticRefundByOutRefundNumberAsync(request);
                    if (result.IsSuccessful())  // 退款成功
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelErrMsg = result.ErrorMessage;
                    }
                }
                return channelRetMsg;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "微信退款查询Exception异常: ");
                return ChannelRetMsg.SysError(e.Message);
            }
        }

        /// <summary>
        /// 微信退款接口
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <param name="refundOrder"></param>
        /// <param name="payOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<ChannelRetMsg> RefundAsync(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                ChannelRetMsg channelRetMsg = new ChannelRetMsg();
                WxServiceWrapper wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);

                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion)) // V2
                {
                    CreatePayRefundV2Request request = new CreatePayRefundV2Request();

                    //放置isv信息
                    //不是特约商户，无需放置此值
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        WxPayIsvSubMchParams isvsubMchParams =
                            (WxPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.SubAppId = isvsubMchParams.SubMchAppId;
                    }

                    request.OutTradeNumber = payOrder.PayOrderId; // 商户订单号
                    request.OutRefundNumber = refundOrder.RefundOrderId; // 退款单号
                    request.TotalFee = (int)payOrder.Amount; // 订单总金额
                    request.RefundFee = (int)refundOrder.RefundAmount; // 退款金额
                    request.NotifyUrl = GetNotifyUrl(refundOrder.RefundOrderId); // 回调url

                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    var result = await client.ExecuteCreatePayRefundV2Async(request);

                    if (result.IsSuccessful()) // 退款发起成功,结果主动查询
                    {
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelOrderId = result.RefundId;
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = result.ErrorCode;
                        channelRetMsg.ChannelErrMsg = WxPayKit.AppendErrMsg(result.ReturnMessage, result.ErrorCodeDescription);
                    }
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion)) // V3
                {
                    CreateRefundDomesticRefundRequest request = new CreateRefundDomesticRefundRequest();
                    request.OutTradeNumber = refundOrder.PayOrderId; // 订单号
                    request.OutRefundNumber = refundOrder.RefundOrderId; // 退款订单号
                    request.NotifyUrl = GetNotifyUrl(refundOrder.RefundOrderId); // 回调地址

                    request.Amount.Refund = (int)refundOrder.RefundAmount; // 退款金额
                    request.Amount.Total = (int)payOrder.Amount; // 订单总金额
                    request.Amount.Currency = "CNY"; // 币种

                    if (mchAppConfigContext.IsIsvSubMch()) // 特约商户
                    {
                        WxPayIsvSubMchParams isvsubMchParams = (WxPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                        request.SubMerchantId = isvsubMchParams.SubMchId;
                    }

                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    var result = await client.ExecuteCreateRefundDomesticRefundAsync(request);
                    string status = result.Status;
                    if ("SUCCESS".Equals(status)) // 退款成功
                    {
                        string refundId = result.RefundId;
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                        channelRetMsg.ChannelOrderId = refundId;
                    }
                    else if ("PROCESSING".Equals(status)) // 退款处理中
                    {
                        string refundId = result.RefundId;
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.ChannelOrderId = refundId;
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrMsg = result.ErrorMessage;
                    }
                }

                return channelRetMsg;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "微信退款Exception异常: ");
                return ChannelRetMsg.SysError(e.Message);
            }
        }
    }
}
