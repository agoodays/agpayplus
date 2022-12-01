using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Events;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Events;
using System.Text.Json;
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
            ConfigContextQueryService configContextQueryService,
            IPayOrderService payOrderService)
            : base(logger, configContextQueryService)
        {
            this.logger = logger;
            this.payOrderService = payOrderService;
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
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
                    string callbackJson = GetReqParamFromBody(request);
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
                    string callbackXml = GetReqParamFromBody(request);
                    if (string.IsNullOrWhiteSpace(callbackXml))
                    {
                        return null;
                    }
                    string callbackJson = XmlUtil.ConvertToJson(callbackXml);
                    var callbackResource = JsonSerializer.Deserialize<OrderEvent>(callbackJson);
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

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }
    }
}
