using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Aop.Api.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 支付宝 回调接口实现类
    /// </summary>
    public class AliPayChannelNoticeService : AbstractChannelNoticeService
    {
        public AliPayChannelNoticeService(ILogger<AliPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public AliPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = await GetReqParamJsonAsync();
                string payOrderId = @params.GetValue("out_trade_no").ToString();
                return new Dictionary<string, object>() { { payOrderId, @params } };
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
                //配置参数获取
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
                JObject jsonParams = JObject.FromObject(@params);

                bool verifyResult = false;
                if (useCert != null && useCert == CS.YES)
                {
                    //证书方式
                    verifyResult = AlipaySignature.RSACertCheckV1(jsonParams.ToKeyValue(), GetCertFilePath(alipayPublicCert), AliPayConfig.CHARSET, alipaySignType);
                }
                else
                {
                    verifyResult = AlipaySignature.RSACheckV1(jsonParams.ToKeyValue(), alipayPublicKey, AliPayConfig.CHARSET, alipaySignType, false);
                }

                //验签失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }

                //验签成功后判断上游订单状态
                ContentResult okResponse = TextResp("SUCCESS");

                ChannelRetMsg result = new ChannelRetMsg();
                result.ChannelOrderId = jsonParams.GetValue("trade_no").ToString(); //渠道订单号
                result.ChannelUserId = jsonParams.GetValue("buyer_id").ToString(); //支付用户ID
                result.ResponseEntity = okResponse; //响应数据

                result.ChannelState = ChannelState.WAITING; // 默认支付中

                if ("TRADE_SUCCESS".Equals(jsonParams.GetValue("trade_status").ToString()))
                {
                    result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                }
                else if ("TRADE_CLOSED".Equals(jsonParams.GetValue("trade_status").ToString()))
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                }

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
