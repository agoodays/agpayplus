using AGooday.AgPay.Application.DataTransfer;
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
using WechatTenpayResponse = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayResponse;

namespace AGooday.AgPay.Components.Third.Channel.WxPay
{
    /// <summary>
    /// 微信关单接口
    /// </summary>
    public class WxPayPayOrderCloseService : IPayOrderCloseService
    {
        private readonly ILogger<WxPayPayOrderCloseService> _logger;
        private readonly ConfigContextQueryService configContextQueryService;

        public WxPayPayOrderCloseService(ILogger<WxPayPayOrderCloseService> logger, ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            this.configContextQueryService = configContextQueryService;
        }

        public WxPayPayOrderCloseService()
        {
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public ChannelRetMsg Close(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                WxServiceWrapper wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion)) // V2
                {
                    ClosePayOrderRequest request = new ClosePayOrderRequest();

                    //放置isv信息
                    //不是特约商户，无需放置此值
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        WxPayIsvSubMchParams isvsubMchParams = (WxPayIsvSubMchParams)configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.SubAppId = isvsubMchParams.SubMchAppId;
                    }

                    request.OutTradeNumber = payOrder.PayOrderId;

                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    var result = client.ExecuteClosePayOrderAsync(request).Result;

                    if ("SUCCESS".Equals(result.ResultCode)) //if (result.IsSuccessful()) //关闭订单成功
                    {
                        return ChannelRetMsg.ConfirmSuccess(null);
                    }
                    else if ("FAIL".Equals(result.ResultCode)) //关闭订单失败
                    {
                        return ChannelRetMsg.ConfirmFail(); //关闭失败
                    }
                    else
                    {
                        return ChannelRetMsg.Waiting(); //关闭中
                    }
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion)) // V3
                {
                    WechatTenpayResponse result;
                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    if (mchAppConfigContext.IsIsvSubMch()) // Sub-merchant
                    {
                        WxPayIsvSubMchParams isvsubMchParams = (WxPayIsvSubMchParams)configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                        ClosePayPartnerTransactionRequest request = new ClosePayPartnerTransactionRequest();
                        request.MerchantId = wxServiceWrapper.Config.MchId;
                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.OutTradeNumber = payOrder.PayOrderId;
                        result = client.ExecuteClosePayPartnerTransactionAsync(request).Result;
                    }
                    else
                    {
                        ClosePayTransactionRequest request = new ClosePayTransactionRequest();
                        request.MerchantId = wxServiceWrapper.Config.MchId;
                        request.OutTradeNumber = payOrder.PayOrderId;
                        result = client.ExecuteClosePayTransactionAsync(request).Result;
                    }

                    if (result.IsSuccessful())
                    {
                        return ChannelRetMsg.ConfirmSuccess(null);//关闭订单成功
                    }
                    else
                    {
                        return ChannelRetMsg.ConfirmFail(); //关闭订单失败
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
