using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IApplymentChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.WxPay
{
    /// <summary>
    /// 微信 V3 进件回调通知实现
    ///
    /// 微信商户平台审核完成后，会通过 V3 证书验签方式将通知 POST 到 notify_url。
    /// 回调类型: "sys.applyment"（进件审核结果通知）
    /// </summary>
    public class WxPayApplymentChannelNoticeService : AbstractApplymentChannelNoticeService
    {
        public WxPayApplymentChannelNoticeService(ILogger<WxPayApplymentChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public WxPayApplymentChannelNoticeService() : base() { }

        public override string GetIfCode() => CS.IF_CODE.WXPAY;

        public override async Task<Dictionary<string, object>> ParseParamsAsync(
            HttpRequest request, string urlApplyId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                string body = await GetReqParamFromBodyAsync();
                if (string.IsNullOrEmpty(body))
                {
                    _logger.LogError("微信进件回调请求体为空");
                    throw new BizException("request body is empty");
                }

                var jObject = JObject.Parse(body);
                var resource = jObject["resource"] as JObject;
                string applymentId = resource?["applyment_id"]?.ToString()
                    ?? jObject["applyment_id"]?.ToString();

                if (string.IsNullOrEmpty(applymentId))
                {
                    _logger.LogError("微信进件回调无法获取 applyment_id");
                    throw new BizException("applyment_id not found");
                }

                return new Dictionary<string, object> { { applymentId, jObject } };
            }
            catch (BizException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "微信进件回调参数解析异常");
                throw new BizException($"参数解析异常: {ex.Message}");
            }
        }

        public override Task<ApplymentNoticeResult> DoNoticeAsync(
            HttpRequest request, object @params, MchApplyDto mchApply,
            IsvConfigContext isvConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                var jObject = (JObject)@params;
                var resource = jObject["resource"] as JObject;
                string applymentId = resource?["applyment_id"]?.ToString()
                    ?? jObject["applyment_id"]?.ToString();

                if (string.IsNullOrEmpty(applymentId))
                    throw new BizException("applyment_id not found");

                if (!string.Equals(applymentId, mchApply.ChannelApplyNo, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("微信进件回调 applyment_id 不匹配: callback={Callback}, local={Local}",
                        applymentId, mchApply.ChannelApplyNo);
                }

                string state = resource?["state"]?.ToString() ?? jObject["state"]?.ToString();
                _logger.LogInformation("微信进件回调 | applymentId={ApplymentId} | state={State}", applymentId, state);

                string jsonResp = "{\"code\":\"SUCCESS\",\"message\":\"成功\"}";
                var result = new ApplymentNoticeResult
                {
                    ChannelApplyNo = applymentId,
                    ChannelOriginResponse = jObject.ToString(),
                };

                switch (state?.ToUpper())
                {
                    case "SUCCESS":
                        result.State = ApplymentNoticeState.CONFIRM_SUCCESS;
                        result.ChannelMchId = resource?["sub_mch_id"]?.ToString();
                        break;
                    case "REJECTED":
                        result.State = ApplymentNoticeState.CONFIRM_FAIL;
                        result.ErrCode = "REJECTED";
                        result.ErrMsg = resource?["reject_reason"]?.ToString() ?? jObject["reject_reason"]?.ToString();
                        break;
                    case "CANCELED":
                        result.State = ApplymentNoticeState.CONFIRM_FAIL;
                        result.ErrCode = "CANCELED";
                        result.ErrMsg = "进件已撤销";
                        break;
                    default:
                        _logger.LogWarning("微信进件回调未知状态: {State}", state);
                        result.State = ApplymentNoticeState.WAITING;
                        break;
                }

                result.ResponseEntity = TextResp(jsonResp);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "微信进件回调处理异常");
                throw;
            }
        }

        public override ActionResult DoNotifyApplyNotExists(HttpRequest request)
            => TextResp("{\"code\":\"FAIL\",\"message\":\"apply not exists\"}");

        public override ActionResult DoNotifyApplyStateUpdateFail(HttpRequest request)
            => TextResp("{\"code\":\"FAIL\",\"message\":\"update state error\"}");
    }
}
