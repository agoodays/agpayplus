using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Aop.Api.Util;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay
{
    public class AliPayTransferNoticeService : AbstractTransferNoticeService
    {
        public AliPayTransferNoticeService(ILogger<AbstractChannelNoticeService> log,
            RequestKit requestKit, ChannelCertConfigKit channelCertConfigKit, ConfigContextQueryService configContextQueryService)
            : base(log, requestKit, channelCertConfigKit, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId)
        {
            try
            {
                var paramsJson = GetReqParamJSON();
                log.LogInformation($"【支付宝转账】回调通知参数：{paramsJson.ToString()}");

                var bizContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(paramsJson["biz_content"].ToString());

                string transferId = bizContent["out_biz_no"];
                return new Dictionary<string, object>() { { transferId, paramsJson } };
            }
            catch (Exception e)
            {
                log.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object parameters, TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【支付宝转账通知】";
            try
            {
                // 配置参数获取
                byte? useCert = null;
                string alipaySignType, alipayPublicCert, alipayPublicKey = null;

                if (mchAppConfigContext.IsIsvSubMch())
                {
                    // 获取支付参数
                    var alipayParams = (AliPayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());
                    useCert = alipayParams.UseCert;
                    alipaySignType = alipayParams.SignType;
                    alipayPublicCert = alipayParams.AlipayPublicCert;
                    alipayPublicKey = alipayParams.AlipayPublicKey;
                }
                else
                {
                    // 获取支付参数
                    var alipayParams = (AliPayNormalMchParams)configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                    useCert = alipayParams.UseCert;
                    alipaySignType = alipayParams.SignType;
                    alipayPublicCert = alipayParams.AlipayPublicCert;
                    alipayPublicKey = alipayParams.AlipayPublicKey;
                }

                // 获取请求参数
                var jsonParams = (Dictionary<string, object>)parameters;
                var bizContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonParams["biz_content"].ToString());

                bool verifyResult;
                if (useCert != null && useCert == CS.YES) // 证书方式
                {
                    verifyResult = AlipaySignature.RSACertCheckV1(jsonParams.ToKeyValue(), GetCertFilePath(alipayPublicCert), AliPayConfig.CHARSET, alipaySignType);
                }
                else
                {
                    verifyResult = AlipaySignature.RSACheckV1(jsonParams.ToKeyValue(), alipayPublicKey, AliPayConfig.CHARSET, alipaySignType, false);
                }

                // 验签失败
                if (!verifyResult)
                {
                    log.LogError($"{logPrefix}，验签失败");
                    throw ResponseException.BuildText("ERROR");
                }

                // 验签成功后判断上游订单状态
                var okResponse = TextResp("SUCCESS");

                var channelRetMsg = new ChannelRetMsg();
                channelRetMsg.ResponseEntity = okResponse; // 响应数据

                channelRetMsg.ChannelState = ChannelState.WAITING; // 默认转账中

                // 成功－SUCCESS
                string status = bizContent["status"];
                if ("SUCCESS".Equals(status))
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                }

                return channelRetMsg;
            }
            catch (Exception e)
            {
                log.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }
    }
}
