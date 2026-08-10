using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.LklPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Channel.LklPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IApplymentChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.LklPay
{
    public class LklPayApplymentChannelNoticeService : AbstractApplymentChannelNoticeService
    {
        public LklPayApplymentChannelNoticeService(
            ILogger<LklPayApplymentChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public LklPayApplymentChannelNoticeService() : base() { }

        public override string GetIfCode() => CS.IF_CODE.LKLPAY;

        public override async Task<Dictionary<string, object>> ParseParamsAsync(
            HttpRequest request, string urlApplyId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                string body = await GetReqParamFromBodyAsync();
                if (string.IsNullOrEmpty(body))
                {
                    _logger.LogError("拉卡拉进件回调请求体为空");
                    throw new BizException("request body is empty");
                }

                var headers = request.Headers.ToDictionary(
                    k => k.Key,
                    v => v.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase);

                var jObject = JObject.Parse(body);
                string applyId = jObject["out_merchant_no"]?.ToString();

                if (string.IsNullOrEmpty(applyId))
                {
                    _logger.LogError("拉卡拉进件回调无法获取 out_merchant_no");
                    throw new BizException("out_merchant_no not found");
                }

                return new Dictionary<string, object>
                {
                    { "applyId", applyId },
                    { "jObject", jObject },
                    { "headers", headers }
                };
            }
            catch (BizException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "拉卡拉进件回调参数解析异常");
                throw new BizException($"参数解析异常: {ex.Message}");
            }
        }

        public override Task<ApplymentNoticeResult> DoNoticeAsync(
            HttpRequest request, object @params, MchApplyDto mchApply,
            IsvConfigContext isvConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                var dict = (Dictionary<string, object>)@params;
                var jObject = (JObject)dict["jObject"];
                var headers = (Dictionary<string, string>)dict["headers"];

                string applyId = jObject["out_merchant_no"]?.ToString();
                if (string.IsNullOrEmpty(applyId))
                    throw new BizException("out_merchant_no not found");

                if (!string.Equals(applyId, mchApply.ApplyId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("拉卡拉进件回调 applyId 不匹配: callback={Callback}, local={Local}",
                        applyId, mchApply.ApplyId);
                }

                var isvParams = isvConfigContext?.GetIsvParamsByIfCode<LklPayIsvParams>(CS.IF_CODE.LKLPAY);
                if (isvParams == null)
                    throw new BizException("拉卡拉通道配置不存在");

                if (!LklPaySignUtil.NoticeVerify(headers, jObject.ToString(), isvParams.PublicCert))
                {
                    _logger.LogWarning("拉卡拉进件回调验签失败 | applyId={ApplyId}", applyId);
                }

                string auditState = jObject["audit_state"]?.ToString();
                string merchantNo = jObject["merchant_no"]?.ToString();
                string applyNo = jObject["apply_no"]?.ToString();

                _logger.LogInformation("拉卡拉进件回调 | applyId={ApplyId} | audit_state={State} | merchant_no={MchNo}",
                    applyId, auditState, merchantNo);

                var result = new ApplymentNoticeResult
                {
                    ChannelApplyNo = applyNo,
                    ChannelOriginResponse = jObject.ToString(),
                };

                switch (auditState?.ToUpper())
                {
                    case "APPROVED":
                    case "SUCCESS":
                        result.State = ApplymentNoticeState.CONFIRM_SUCCESS;
                        result.ChannelMchId = merchantNo;
                        break;
                    case "REJECTED":
                    case "FAIL":
                        result.State = ApplymentNoticeState.CONFIRM_FAIL;
                        result.ErrCode = auditState;
                        result.ErrMsg = jObject["audit_desc"]?.ToString() ?? "审核拒绝";
                        break;
                    case "PENDING":
                    case "AUDITING":
                    case "PROCESSING":
                        result.State = ApplymentNoticeState.WAITING;
                        break;
                    default:
                        _logger.LogWarning("拉卡拉进件回调未知状态: {AuditState}", auditState);
                        result.State = ApplymentNoticeState.WAITING;
                        break;
                }

                result.ResponseEntity = TextResp("{\"code\":\"000000\",\"msg\":\"success\"}");
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "拉卡拉进件回调处理异常");
                throw;
            }
        }

        public override ActionResult DoNotifyApplyNotExists(HttpRequest request)
            => TextResp("{\"code\":\"400001\",\"msg\":\"apply not exists\"}");

        public override ActionResult DoNotifyApplyStateUpdateFail(HttpRequest request)
            => TextResp("{\"code\":\"400002\",\"msg\":\"update state error\"}");
    }
}
