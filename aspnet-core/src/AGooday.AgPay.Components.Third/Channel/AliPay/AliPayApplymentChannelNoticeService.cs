using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IApplymentChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 支付宝服务商模式进件回调通知实现
    ///
    /// 支付宝服务商调用 ant.merchant.expand.indirect 审核完成后，
    /// 支付宝会将结果通知到 notify_url。
    ///
    /// 回调参数（表单 POST）:
    ///   merchant_notify_content — 包含 sub_merchant_id / audit_result / fail_reason 等
    ///   sign / sign_type — 验签字段
    /// </summary>
    public class AliPayApplymentChannelNoticeService : AbstractApplymentChannelNoticeService
    {
        public AliPayApplymentChannelNoticeService(ILogger<AliPayApplymentChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public AliPayApplymentChannelNoticeService() : base() { }

        public override string GetIfCode() => CS.IF_CODE.ALIPAY;

        public override async Task<Dictionary<string, object>> ParseParamsAsync(
            HttpRequest request, string urlApplyId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                string body = await GetReqParamFromBodyAsync();
                if (string.IsNullOrEmpty(body))
                {
                    _logger.LogError("支付宝进件回调请求体为空");
                    throw new BizException("request body is empty");
                }

                JObject jObject;
                if (body.TrimStart().StartsWith("{"))
                {
                    jObject = JObject.Parse(body);
                }
                else
                {
                    jObject = ParseFormToJObject(body);
                }

                string notifyContent = jObject["merchant_notify_content"]?.ToString();
                if (!string.IsNullOrEmpty(notifyContent))
                {
                    notifyContent = Uri.UnescapeDataString(notifyContent);
                    var inner = JObject.Parse(notifyContent);
                    foreach (var prop in inner.Properties())
                    {
                        jObject[prop.Name] = prop.Value;
                    }
                }

                string subMerchantId = jObject["sub_merchant_id"]?.ToString();
                if (string.IsNullOrEmpty(subMerchantId))
                {
                    _logger.LogError("支付宝进件回调无法获取 sub_merchant_id");
                    throw new BizException("sub_merchant_id not found");
                }

                return new Dictionary<string, object> { { subMerchantId, jObject } };
            }
            catch (BizException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付宝进件回调参数解析异常");
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
                string subMerchantId = jObject["sub_merchant_id"]?.ToString();
                string auditResult = jObject["audit_result"]?.ToString();
                string failReason = jObject["fail_reason"]?.ToString();

                _logger.LogInformation("支付宝进件回调 | subMerchantId={SubMchId} | auditResult={Result} | failReason={Reason}",
                    subMerchantId, auditResult, failReason);

                var result = new ApplymentNoticeResult
                {
                    ChannelApplyNo = subMerchantId,
                    ChannelOriginResponse = jObject.ToString(),
                };

                switch (auditResult?.ToUpper())
                {
                    case "AUDIT_SUCCESS":
                        result.State = ApplymentNoticeState.CONFIRM_SUCCESS;
                        result.ChannelMchId = subMerchantId;
                        break;
                    case "AUDIT_FAIL":
                        result.State = ApplymentNoticeState.CONFIRM_FAIL;
                        result.ErrCode = "AUDIT_FAIL";
                        result.ErrMsg = failReason ?? "审核未通过";
                        break;
                    default:
                        _logger.LogWarning("支付宝进件回调未知状态: {AuditResult}", auditResult);
                        result.State = ApplymentNoticeState.WAITING;
                        break;
                }

                result.ResponseEntity = TextResp("success");
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付宝进件回调处理异常");
                throw;
            }
        }

        public override ActionResult DoNotifyApplyNotExists(HttpRequest request) => TextResp("success");
        public override ActionResult DoNotifyApplyStateUpdateFail(HttpRequest request) => TextResp("success");

        private static JObject ParseFormToJObject(string formBody)
        {
            var result = new JObject();
            foreach (var pair in formBody.Split('&'))
            {
                int idx = pair.IndexOf('=');
                if (idx > 0)
                {
                    string key = pair.Substring(0, idx);
                    string val = pair.Substring(idx + 1);
                    result[key] = Uri.UnescapeDataString(val);
                }
            }
            return result;
        }
    }
}
