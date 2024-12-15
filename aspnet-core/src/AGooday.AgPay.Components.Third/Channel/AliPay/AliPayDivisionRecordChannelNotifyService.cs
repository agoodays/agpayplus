using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Aop.Api.Util;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 支付宝 分账回调接口实现类
    /// 
    /// 注意：
    /// 
    ///  royalty_mode 必须传入：  async ( 使用异步：  需要 1、请配置 支付宝应用的网关地址 （ xxx.com/api/channelbiz/alipay/appGatewayMsgReceive ）， 2、 订阅消息。   )
    ///  2023-03-30 咨询支付宝客服：  如果没有传royalty_mode分账模式,这个默认会是同步分账,同步分账不需要关注异步通知,接口调用成功就分账成功了  2,同步分账默认不会给您发送异步通知。
    ///  3. 服务商代商户调用商家分账，当异步分账时服务商必须调用alipay.open.app.message.topic.subscribe(订阅消息主题)对消息api做关联绑定，服务商才会收到alipay.trade.order.settle.notify通知，否则服务商无法收到通知。
    ///  https://opendocs.alipay.com/open/20190308105425129272/quickstart#%E8%9A%82%E8%9A%81%E6%B6%88%E6%81%AF%EF%BC%9A%E4%BA%A4%E6%98%93%E5%88%86%E8%B4%A6%E7%BB%93%E6%9E%9C%E9%80%9A%E7%9F%A5
    /// </summary>
    public class AliPayDivisionRecordChannelNotifyService : AbstractDivisionRecordChannelNotifyService
    {
        public AliPayDivisionRecordChannelNotifyService(ILogger<AliPayDivisionRecordChannelNotifyService> logger,
            RequestKit requestKit, ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public AliPayDivisionRecordChannelNotifyService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request)
        {
            try
            {
                JObject paramsJson = await GetReqParamJSONAsync();
                string batchOrderId = paramsJson["biz_content"]["out_request_no"].ToString(); // 分账批次号
                return new Dictionary<string, object>() { { batchOrderId, paramsJson } };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override async Task<DivisionChannelNotifyModel> DoNotifyAsync(HttpRequest request, object parameters, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext)
        {
            // 响应结果
            DivisionChannelNotifyModel result = new DivisionChannelNotifyModel();
            try
            {
                // 配置参数获取
                byte? useCert = null;
                string alipaySignType, alipayPublicCert, alipayPublicKey = null;

                if (mchAppConfigContext.IsIsvSubMch())
                {
                    // 获取支付参数
                    AliPayIsvParams alipayParams = (AliPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());
                    useCert = alipayParams.UseCert;
                    alipaySignType = alipayParams.SignType;
                    alipayPublicCert = alipayParams.AlipayPublicCert;
                    alipayPublicKey = alipayParams.AlipayPublicKey;
                }
                else
                {
                    // 获取支付参数
                    AliPayNormalMchParams alipayParams = (AliPayNormalMchParams)await _configContextQueryService.QueryNormalMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                    useCert = alipayParams.UseCert;
                    alipaySignType = alipayParams.SignType;
                    alipayPublicCert = alipayParams.AlipayPublicCert;
                    alipayPublicKey = alipayParams.AlipayPublicKey;
                }

                // 获取请求参数
                JObject jsonParams = (JObject)parameters;

                bool verifyResult;
                if (useCert != null && useCert == CS.YES) //证书方式
                {
                    verifyResult = AlipaySignature.RSACertCheckV1(jsonParams.ToObject<Dictionary<string, string>>(), GetCertFilePath(alipayPublicCert), AliPayConfig.CHARSET, alipaySignType);
                }
                else
                {
                    verifyResult = AlipaySignature.RSACheckV1(jsonParams.ToObject<Dictionary<string, string>>(), alipayPublicKey, AliPayConfig.CHARSET, alipaySignType, false);
                }

                //验签失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }

                // 得到所有的 accNo 与 recordId map
                Dictionary<string, long?> accnoAndRecordIdSet = new Dictionary<string, long?>();
                foreach (PayOrderDivisionRecordDto record in recordList)
                {
                    accnoAndRecordIdSet.Add(record.AccNo, record.RecordId);
                }

                Dictionary<long, ChannelRetMsg> recordResultMap = new Dictionary<long, ChannelRetMsg>();

                JObject bizContentJSON = jsonParams["biz_content"].ToObject<JObject>();

                // 循环
                JArray array = bizContentJSON["royalty_detail_list"].ToObject<JArray>();
                foreach (JToken item in array)
                {
                    JObject itemJSON = (JObject)item;

                    // 我方系统的分账接收记录ID
                    long? recordId = accnoAndRecordIdSet.GetValueOrDefault(itemJSON.Value<string>("trans_in"));

                    // 分账类型 && 包含该笔分账账号
                    if ("transfer".Equals(itemJSON.Value<string>("operation_type")) && recordId != null)
                    {
                        // 分账成功
                        if ("SUCCESS".Equals(itemJSON.Value<string>("state")))
                        {
                            recordResultMap.Add(recordId.Value, ChannelRetMsg.ConfirmSuccess(bizContentJSON.Value<string>("settle_no")));
                        }

                        // 分账失败
                        if ("FAIL".Equals(itemJSON.Value<string>("state")))
                        {
                            recordResultMap.Add(recordId.Value, ChannelRetMsg.ConfirmFail(bizContentJSON.Value<string>("settle_no"), itemJSON.Value<string>("error_code"), itemJSON.Value<string>("error_desc")));
                        }
                    }
                }

                result.RecordResultMap = recordResultMap;
                result.ApiRes = TextResp("success");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }
    }
}
