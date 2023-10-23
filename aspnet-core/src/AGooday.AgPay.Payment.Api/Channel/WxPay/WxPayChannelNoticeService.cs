using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Events;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Events;
using static AGooday.AgPay.Payment.Api.Channel.IChannelNoticeService;
using WechatTenpayClientV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClient;
using WechatTenpayClientV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClient;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    /// <summary>
    /// 微信回调
    /// </summary>
    public class WxPayChannelNoticeService : AbstractChannelNoticeService
    {
        private readonly IPayOrderService payOrderService;
        private readonly ILogger<WxPayChannelNoticeService> logger;

        public WxPayChannelNoticeService(ILogger<WxPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService,
            IPayOrderService payOrderService)
            : base(logger, requestKit, configContextQueryService)
        {
            this.logger = logger;
            this.payOrderService = payOrderService;
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                // V3接口回调
                if (!string.IsNullOrEmpty(urlOrderId))
                {
                    // 获取订单信息
                    PayOrderDto payOrder = payOrderService.GetById(urlOrderId);
                    if (payOrder == null)
                    {
                        throw new BizException("订单不存在");
                    }

                    //获取支付参数 (缓存数据) 和 商户信息
                    MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId);
                    if (mchAppConfigContext == null)
                    {
                        throw new BizException("获取商户信息失败");
                    }

                    // 验签 && 获取订单回调数据
                    var wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);
                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    /* 微信商户平台发来的通知内容 */
                    var timestamp = request.Headers["Wechatpay-Timestamp"].FirstOrDefault();
                    var nonce = request.Headers["Wechatpay-Nonce"].FirstOrDefault();
                    var signature = request.Headers["Wechatpay-Signature"].FirstOrDefault();
                    var serialNumber = request.Headers["Wechatpay-Serial"].FirstOrDefault();
                    string callbackJson = GetReqParamFromBody();

                    JObject headerJSON = new JObject();
                    headerJSON.Add("Wechatpay-Timestamp", timestamp);
                    headerJSON.Add("Wechatpay-Nonce", nonce);
                    headerJSON.Add("Wechatpay-Signature", signature);
                    headerJSON.Add("Wechatpay-Serial", serialNumber);
                    log.LogInformation($"\n【请求头信息】：{headerJSON}\n【加密数据】：{callbackJson}");

                    bool valid = client.VerifyEventSignature(
                        callbackTimestamp: timestamp,
                        callbackNonce: nonce,
                        callbackBody: callbackJson,
                        callbackSignature: signature,
                        callbackSerialNumber: serialNumber, out Exception error);
                    if (!valid)
                    {
                        log.LogError(error, "error");
                        throw ResponseException.BuildText("ERROR");
                    }
                    /* 将 JSON 反序列化得到通知对象 */
                    var callbackModel = client.DeserializeEvent(callbackJson);
                    if ("TRANSACTION.SUCCESS".Equals(callbackModel.EventType))
                    {
                        /* 根据事件类型，解密得到支付通知敏感数据 */
                        if (mchAppConfigContext.IsIsvSubMch())
                        {
                            var callbackResource = client.DecryptEventResource<PartnerTransactionResource>(callbackModel);
                            var payOrderId = callbackResource.OutTradeNumber;
                            return new Dictionary<string, object>() { { payOrderId, callbackResource } };
                        }
                        else
                        {
                            var callbackResource = client.DecryptEventResource<TransactionResource>(callbackModel);
                            var payOrderId = callbackResource.OutTradeNumber;
                            return new Dictionary<string, object>() { { payOrderId, callbackResource } };
                        }
                    }
                }
                // V2接口回调
                else
                {
                    string callbackXml = GetReqParamFromBody();
                    if (string.IsNullOrWhiteSpace(callbackXml))
                    {
                        return null;
                    }
                    string callbackJson = XmlUtil.ConvertToJson(callbackXml);
                    var callbackResource = JsonConvert.DeserializeObject<OrderEvent>(callbackJson);
                    var payOrderId = callbackResource.OutTradeNumber;
                    return new Dictionary<string, object>() { { payOrderId, callbackResource } };
                }
                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg channelResult = new ChannelRetMsg();
                channelResult.ChannelState = ChannelState.WAITING; // 默认支付中

                var wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

                // V2
                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    // 获取回调参数
                    //var result = (OrderEvent)@params;
                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    string callbackXml = GetReqParamFromBody();
                    var result = client.JsonSerializer.Deserialize<OrderEvent>(callbackXml);
                    // 验证参数
                    bool valid = client.VerifyEventSignature(callbackXml, out Exception error);
                    if (!valid)
                    {
                        log.LogError(error, "error");
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
                logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }
    }
}
