using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.WxPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Events;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Events;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;
using WechatTenpayClientV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClient;
using WechatTenpayClientV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClient;

namespace AGooday.AgPay.Components.Third.Channel.WxPay
{
    /// <summary>
    /// 微信回调
    /// </summary>
    public class WxPayChannelNoticeService : AbstractChannelNoticeService
    {
        private readonly IPayOrderService _payOrderService;

        public WxPayChannelNoticeService(ILogger<WxPayChannelNoticeService> logger,
            IPayOrderService payOrderService,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
            _payOrderService = payOrderService;
        }

        public WxPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                // V3接口回调
                if (!string.IsNullOrEmpty(urlOrderId))
                {
                    // 获取订单信息
                    PayOrderDto payOrder = await _payOrderService.GetByIdAsync(urlOrderId);
                    if (payOrder == null)
                    {
                        throw new BizException("订单不存在");
                    }

                    //获取支付参数 (缓存数据) 和 商户信息
                    MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(payOrder.MchNo, payOrder.AppId);
                    if (mchAppConfigContext == null)
                    {
                        throw new BizException("获取商户信息失败");
                    }

                    // 验签 && 获取订单回调数据
                    var wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);
                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    /* 微信商户平台发来的通知内容 */
                    var timestamp = request.Headers["Wechatpay-Timestamp"].FirstOrDefault();
                    var nonce = request.Headers["Wechatpay-Nonce"].FirstOrDefault();
                    var signature = request.Headers["Wechatpay-Signature"].FirstOrDefault();
                    var serialNumber = request.Headers["Wechatpay-Serial"].FirstOrDefault();
                    string webhookJson = await GetReqParamFromBodyAsync();

                    JObject headerJSON = new JObject();
                    headerJSON.Add("Wechatpay-Timestamp", timestamp);
                    headerJSON.Add("Wechatpay-Nonce", nonce);
                    headerJSON.Add("Wechatpay-Signature", signature);
                    headerJSON.Add("Wechatpay-Serial", serialNumber);
                    _logger.LogInformation("\n【请求头信息】：{headerJSON}\n【加密数据】：{webhookJson}", headerJSON, webhookJson);
                    //_logger.LogInformation($"\n【请求头信息】：{headerJSON}\n【加密数据】：{webhookJson}");

                    var valid = client.VerifyEventSignature(timestamp, nonce, webhookJson, signature, serialNumber);
                    if (!valid.Result)
                    {
                        _logger.LogError(valid.Error, "error");
                        throw ResponseException.BuildText("ERROR");
                    }
                    /* 将 JSON 反序列化得到通知对象 */
                    var webhookModel = client.DeserializeEvent(webhookJson);
                    if ("TRANSACTION.SUCCESS".Equals(webhookModel.EventType))
                    {
                        /* 根据事件类型，解密得到支付通知敏感数据 */
                        if (mchAppConfigContext.IsIsvSubMch())
                        {
                            var webhookResource = client.DecryptEventResource<PartnerTransactionResource>(webhookModel);
                            var payOrderId = webhookResource.OutTradeNumber;
                            return new Dictionary<string, object>() { { payOrderId, webhookResource } };
                        }
                        else
                        {
                            var webhookResource = client.DecryptEventResource<TransactionResource>(webhookModel);
                            var payOrderId = webhookResource.OutTradeNumber;
                            return new Dictionary<string, object>() { { payOrderId, webhookResource } };
                        }
                    }
                }
                // V2接口回调
                else
                {
                    string webhookXml = await GetReqParamFromBodyAsync();
                    if (string.IsNullOrWhiteSpace(webhookXml))
                    {
                        return null;
                    }
                    string webhookJson = XmlUtil.ConvertToJson(webhookXml);
                    var webhookResource = JsonConvert.DeserializeObject<OrderEvent>(webhookJson);
                    var payOrderId = webhookResource.OutTradeNumber;
                    return new Dictionary<string, object>() { { payOrderId, webhookResource } };
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override async Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg channelResult = new ChannelRetMsg();
                channelResult.ChannelState = ChannelState.WAITING; // 默认支付中

                var wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);

                // V2
                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    // 获取回调参数
                    //var result = (OrderEvent)@params;
                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    string webhookXml = await GetReqParamFromBodyAsync();
                    var result = client.DeserializeEvent<OrderEvent>(webhookXml);
                    // 验证参数
                    var valid = client.VerifyEventSignature(webhookXml);
                    if (!valid.Result)
                    {
                        _logger.LogError(valid.Error, "error");
                        throw ResponseException.BuildText("ERROR");
                    }
                    // 核对金额
                    long wxPayAmt = result.TotalFee; ;
                    long dbPayAmt = payOrder.Amount;
                    if (dbPayAmt != wxPayAmt)
                    {
                        throw ResponseException.BuildText("AMOUNT ERROR");
                    }

                    channelResult.ChannelOrderId = result.TransactionId; //渠道订单号
                    channelResult.ChannelUserId = result.OpenId; //支付用户ID
                    channelResult.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    channelResult.ResponseEntity = TextResp(WxPayKit.SuccessResp("OK"));
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    // 获取回调参数
                    string channelState = string.Empty;
                    string channelOrderId = string.Empty;
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        var result = (PartnerTransactionResource)@params;
                        channelState = result.TradeState;
                        channelResult.ChannelOrderId = result.TransactionId;//渠道订单号
                        if (result.Payer != null)
                        {
                            channelResult.ChannelUserId = result.Payer.OpenId;//支付用户ID
                        }
                    }
                    else
                    {
                        var result = (TransactionResource)@params;
                        channelState = result.TradeState;
                        channelResult.ChannelOrderId = result.TransactionId;//渠道订单号
                        if (result.Payer != null)
                        {
                            channelResult.ChannelUserId = result.Payer.OpenId;//支付用户ID
                        }
                    }
                    if ("SUCCESS".Equals(channelState))
                    {
                        channelResult.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    }
                    //CLOSED—已关闭， REVOKED—已撤销, PAYERROR--支付失败
                    else if ("CLOSED".Equals(channelState)
                        || "REVOKED".Equals(channelState)
                        || "PAYERROR".Equals(channelState))
                    {
                        channelResult.ChannelState = ChannelState.CONFIRM_FAIL;//支付失败
                    }

                    JObject resJSON = new JObject();
                    resJSON.Add("code", "SUCCESS");
                    resJSON.Add("message", "成功");

                    var okResponse = JsonResp(resJSON);
                    channelResult.ResponseEntity = okResponse;
                }
                else
                {
                    throw ResponseException.BuildText("API_VERSION ERROR");
                }
                return channelResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }
    }
}
