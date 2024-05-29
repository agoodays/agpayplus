using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
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
using static AGooday.AgPay.Payment.Api.Channel.IChannelRefundNoticeService;
using WechatTenpayClientV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClient;
using WechatTenpayClientV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClient;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    /// <summary>
    /// 微信支付 退款回调接口实现类
    /// </summary>
    public class WxPayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        private readonly IRefundOrderService refundOrderService;

        public WxPayChannelRefundNoticeService(ILogger<WxPayChannelRefundNoticeService> logger,
            IRefundOrderService refundOrderService,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
            this.refundOrderService = refundOrderService;
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                // 获取订单信息
                RefundOrderDto refundOrder = refundOrderService.GetById(urlOrderId);
                if (refundOrder == null)
                {
                    throw new BizException("订单不存在");
                }

                // 获取支付参数 (缓存数据) 和 商户信息
                MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(refundOrder.MchNo, refundOrder.AppId);
                if (mchAppConfigContext == null)
                {
                    throw new BizException("获取商户信息失败");
                }
                string apiVersion = ""; // 接口类型
                string wxKey = "";  // 微信私钥
                byte mchType = mchAppConfigContext.MchType;
                if (CS.MCH_TYPE_NORMAL == mchType)
                {
                    WxPayNormalMchParams normalMchParams = (WxPayNormalMchParams)configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                    apiVersion = normalMchParams.ApiVersion;
                    wxKey = CS.PAY_IF_VERSION.WX_V2.Equals(apiVersion) ? normalMchParams.Key : normalMchParams.ApiV3Key;
                }
                else if (CS.MCH_TYPE_ISVSUB == mchType)
                {
                    WxPayIsvParams wxpayIsvParams = (WxPayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());
                    apiVersion = wxpayIsvParams.ApiVersion;
                    wxKey = CS.PAY_IF_VERSION.WX_V2.Equals(apiVersion) ? wxpayIsvParams.Key : wxpayIsvParams.ApiV3Key;
                }
                else
                {
                    throw new BizException("商户类型错误");
                }

                var wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);
                if (CS.PAY_IF_VERSION.WX_V3.Equals(apiVersion)) // V3接口回调
                {
                    // 验签 && 获取订单回调数据
                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    /* 微信商户平台发来的通知内容 */
                    var timestamp = request.Headers["Wechatpay-Timestamp"].FirstOrDefault();
                    var nonce = request.Headers["Wechatpay-Nonce"].FirstOrDefault();
                    var signature = request.Headers["Wechatpay-Signature"].FirstOrDefault();
                    var serialNumber = request.Headers["Wechatpay-Serial"].FirstOrDefault();
                    string webhookJson = GetReqParamFromBody();

                    JObject headerJSON = new JObject();
                    headerJSON.Add("Wechatpay-Timestamp", timestamp);
                    headerJSON.Add("Wechatpay-Nonce", nonce);
                    headerJSON.Add("Wechatpay-Signature", signature);
                    headerJSON.Add("Wechatpay-Serial", serialNumber);
                    _logger.LogInformation($"\n【请求头信息】：{headerJSON}\n【加密数据】：{webhookJson}");

                    var valid = client.VerifyEventSignature(timestamp, nonce, webhookJson, signature, serialNumber);
                    if (!valid.Result)
                    {
                        _logger.LogError(valid.Error, "error");
                        throw ResponseException.BuildText("ERROR");
                    }
                    /* 将 JSON 反序列化得到通知对象 */
                    var webhookModel = client.DeserializeEvent(webhookJson);
                    if ("REFUND.SUCCESS".Equals(webhookModel.EventType))
                    {
                        /* 根据事件类型，解密得到支付通知敏感数据 */
                        if (mchAppConfigContext.IsIsvSubMch())
                        {
                            var webhookResource = client.DecryptEventResource<PartnerRefundResource>(webhookModel);
                            var payOrderId = webhookResource.OutTradeNumber;
                            return new Dictionary<string, object>() { { payOrderId, webhookResource } };
                        }
                        else
                        {
                            var webhookResource = client.DecryptEventResource<RefundResource>(webhookModel);
                            var payOrderId = webhookResource.OutTradeNumber;
                            return new Dictionary<string, object>() { { payOrderId, webhookResource } };
                        }
                    }
                }
                else if (CS.PAY_IF_VERSION.WX_V2.Equals(apiVersion)) // V2接口回调
                {
                    // 验签 && 获取订单回调数据
                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    string webhookXml = GetReqParamFromBody();
                    var valid = client.VerifyEventSignature(webhookXml);
                    if (!valid.Result)
                    {
                        _logger.LogError(valid.Error, "error");
                        throw ResponseException.BuildText("ERROR");
                    }
                    if (string.IsNullOrWhiteSpace(webhookXml))
                    {
                        return null;
                    }
                    string webhookJson = XmlUtil.ConvertToJson(webhookXml);
                    var webhookResource = JsonConvert.DeserializeObject<RefundEvent.Types.RefundInfo>(webhookJson);
                    var payOrderId = webhookResource.OutTradeNumber;
                    return new Dictionary<string, object>() { { urlOrderId, webhookResource } };
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg channelResult = new ChannelRetMsg();
                WxServiceWrapper wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

                channelResult.ChannelState = ChannelState.WAITING; // Default payment in progress
                if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    // 获取回调参数
                    string refundStatus = string.Empty;
                    string channelOrderId = string.Empty;
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        var notifyResult = (PartnerRefundResource)@params;
                        refundStatus = notifyResult.RefundStatus;
                        channelResult.ChannelOrderId = notifyResult.TransactionId;//渠道订单号
                    }
                    else
                    {
                        var notifyResult = (RefundResource)@params;
                        refundStatus = notifyResult.RefundStatus;
                        channelResult.ChannelOrderId = notifyResult.TransactionId;//渠道订单号
                    }

                    if ("SUCCESS".Equals(refundStatus))
                    {
                        channelResult.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    }
                    else
                    {
                        //CHANGE—退款异常， REFUNDCLOSE—退款关闭
                        channelResult.ChannelState = ChannelState.CONFIRM_FAIL; //退款失败
                    }

                    JObject resJSON = new JObject();
                    resJSON.Add("code", "SUCCESS");
                    resJSON.Add("message", "成功");

                    var okResponse = JsonResp(resJSON);
                    channelResult.ResponseEntity = okResponse;
                }
                else if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    // 获取回调参数
                    //var result = (OrderEvent)@params;
                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    string webhookXml = GetReqParamFromBody();
                    var webhookModel = client.DeserializeEvent(webhookXml);
                    var notifyResult = client.DecryptEventRequestInfo<RefundEvent.Types.RefundInfo>(webhookModel);
                    //var notifyResult = client.JsonSerializer.Deserialize<RefundEvent.Types.RefundInfo>(webhookXml);
                    // 验证参数
                    var valid = client.VerifyEventSignature(webhookXml);
                    if (!valid.Result)
                    {
                        _logger.LogError(valid.Error, "error");
                        throw ResponseException.BuildText("ERROR");
                    }
                    // 核对金额
                    long wxPayAmt = notifyResult.TotalFee; ;
                    long dbPayAmt = refundOrder.RefundAmount;
                    if (dbPayAmt != wxPayAmt)
                    {
                        throw ResponseException.BuildText("AMOUNT ERROR");
                    }

                    channelResult.ChannelOrderId = notifyResult.TransactionId; //渠道订单号
                    if ("SUCCESS".Equals(notifyResult.RefundStatus))
                    {
                        channelResult.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    }
                    else
                    {
                        //CHANGE—退款异常， REFUNDCLOSE—退款关闭
                        channelResult.ChannelState = ChannelState.CONFIRM_FAIL; //退款失败
                    }
                    channelResult.ResponseEntity = TextResp(WxPayKit.SuccessResp("OK"));
                }
                else
                {
                    channelResult.ChannelState = ChannelState.CONFIRM_FAIL; //退款失败
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
