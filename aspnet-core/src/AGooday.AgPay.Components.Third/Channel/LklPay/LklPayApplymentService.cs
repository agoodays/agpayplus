using System.Diagnostics;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.LklPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Channel.LklPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Applyment;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.LklPay
{
    public class LklPayApplymentService : AbstractApplymentService
    {
        private const string API_MERCHANT_REGISTER = "/api/v3/labs/merchant/register";
        private const string API_MERCHANT_QUERY = "/api/v3/labs/merchant/query";
        private const string SUCCESS_CODE = "BBS00000";

        public LklPayApplymentService(
            ILogger<LklPayApplymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode() => CS.IF_CODE.LKLPAY;

        public override async Task<ApplymentSubmitRS> SubmitAsync(ApplymentSubmitRQ rq, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentSubmitRS();
            var isvParams = isvCtx?.GetIsvParamsByIfCode<LklPayIsvParams>(CS.IF_CODE.LKLPAY);
            if (isvParams == null)
            {
                result.ErrMsg = "拉卡拉通道配置不存在";
                return result;
            }

            if (string.IsNullOrEmpty(mchApply?.MchFullName))
            {
                result.ErrMsg = "缺少必要参数：商户全称";
                return result;
            }

            try
            {
                var reqData = BuildRegisterReqData(mchApply, isvParams);
                var reqParams = new JObject
                {
                    { "req_time", DateTime.Now.ToString("yyyyMMddHHmmss") },
                    { "version", "3.0" },
                    { "req_data", reqData }
                };
                result.ChannelReqMsg = reqParams.ToString();

                var host = LklPayPaymentService.GetHost4env(isvParams);
                var url = host + API_MERCHANT_REGISTER;
                var stopwatch = Stopwatch.StartNew();

                var (resText, headers) = await LklPayHttpUtil.DoPostJsonAsync(
                    url, isvParams.AppId, isvParams.SerialNo, isvParams.PrivateCert, reqParams);

                stopwatch.Stop();
                _logger.LogInformation("拉卡拉商户进件提交 | applyId={ApplyId} | url={Url} | cost={Cost}ms | req={Req} | res={Res}",
                    mchApply.ApplyId, url, stopwatch.ElapsedMilliseconds, result.ChannelReqMsg, resText);

                if (string.IsNullOrWhiteSpace(resText))
                {
                    result.ErrMsg = "拉卡拉进件响应为空";
                    return result;
                }

                result.ChannelResp = resText;
                var resJson = JObject.Parse(resText);

                if (!LklPaySignUtil.Verify(headers, isvParams.AppId, resText, isvParams.PublicCert))
                {
                    _logger.LogWarning("拉卡拉商户进件提交验签失败 | applyId={ApplyId}", mchApply.ApplyId);
                }

                var code = resJson["code"]?.ToString();
                var msg = resJson["msg"]?.ToString();

                if (SUCCESS_CODE.Equals(code))
                {
                    var respData = resJson["resp_data"] as JObject;
                    result.ChannelMchId = respData?["merchant_no"]?.ToString();
                    result.ChannelApplyNo = respData?["apply_no"]?.ToString();

                    var auditState = respData?["audit_state"]?.ToString();
                    result.ChannelState = auditState switch
                    {
                        "APPROVED" or "SUCCESS" => 1,
                        "REJECTED" => 1,
                        _ => 1
                    };
                }
                else
                {
                    result.ErrMsg = $"拉卡拉进件失败 [{code}] {msg}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "拉卡拉商户进件提交异常 | applyId={ApplyId}", mchApply.ApplyId);
                result.ErrMsg = $"拉卡拉进件异常: {ex.Message}";
            }

            return result;
        }

        public override async Task<ApplymentQueryRS> QueryAsync(string applyId, string channelApplyNo, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentQueryRS { ChannelState = "FAIL" };
            var isvParams = isvCtx?.GetIsvParamsByIfCode<LklPayIsvParams>(CS.IF_CODE.LKLPAY);
            if (isvParams == null)
            {
                result.ChannelStateDesc = "拉卡拉通道配置不存在";
                return result;
            }

            try
            {
                var reqData = new JObject();
                if (!string.IsNullOrEmpty(channelApplyNo))
                    reqData["apply_no"] = channelApplyNo;
                else if (!string.IsNullOrEmpty(channelMchId))
                    reqData["merchant_no"] = channelMchId;
                else
                    reqData["out_merchant_no"] = applyId;

                var reqParams = new JObject
                {
                    { "req_time", DateTime.Now.ToString("yyyyMMddHHmmss") },
                    { "version", "3.0" },
                    { "req_data", reqData }
                };

                var host = LklPayPaymentService.GetHost4env(isvParams);
                var url = host + API_MERCHANT_QUERY;
                var stopwatch = Stopwatch.StartNew();

                var (resText, headers) = await LklPayHttpUtil.DoPostJsonAsync(
                    url, isvParams.AppId, isvParams.SerialNo, isvParams.PrivateCert, reqParams);

                stopwatch.Stop();
                _logger.LogInformation("拉卡拉商户进件查询 | applyId={ApplyId} | url={Url} | cost={Cost}ms | res={Res}",
                    applyId, url, stopwatch.ElapsedMilliseconds, resText);

                if (string.IsNullOrWhiteSpace(resText))
                {
                    result.ChannelStateDesc = "拉卡拉进件查询响应为空";
                    return result;
                }

                result.ChannelResp = resText;
                var resJson = JObject.Parse(resText);

                if (!LklPaySignUtil.Verify(headers, isvParams.AppId, resText, isvParams.PublicCert))
                {
                    _logger.LogWarning("拉卡拉商户进件查询验签失败 | applyId={ApplyId}", applyId);
                }

                var code = resJson["code"]?.ToString();
                var msg = resJson["msg"]?.ToString();

                if (SUCCESS_CODE.Equals(code))
                {
                    var respData = resJson["resp_data"] as JObject;
                    var auditState = respData?["audit_state"]?.ToString();

                    result.ChannelMchId = respData?["merchant_no"]?.ToString();
                    result.ChannelState = MapAuditState(auditState);
                    result.ChannelStateDesc = auditState;
                }
                else
                {
                    result.ChannelStateDesc = $"拉卡拉查询失败 [{code}] {msg}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "拉卡拉商户进件查询异常 | applyId={ApplyId}", applyId);
                result.ChannelStateDesc = $"查询异常: {ex.Message}";
            }

            return result;
        }

        public override Task<ApplymentSubmitRS> GetSignUrlAsync(string applyId, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            return Task.FromResult(new ApplymentSubmitRS
            {
                ErrMsg = "拉卡拉通道进件成功后自动开通，无需单独获取签约链接"
            });
        }

        public override Task<ApplymentQueryRS> VerifyAsync(string applyId, long verifyAmount, string verifyCode, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            return Task.FromResult(new ApplymentQueryRS
            {
                ChannelState = "SUCCESS",
                ChannelStateDesc = "拉卡拉通道由系统自动完成账户验证，无需小额打款"
            });
        }

        #region 请求体构建

        private JObject BuildRegisterReqData(MchApplyDto mchApply, LklPayIsvParams isvParams)
        {
            var reqData = new JObject
            {
                { "out_merchant_no", mchApply.ApplyId },
                { "org_code", isvParams.OrgCode ?? "" },
                { "merchant_name", mchApply.MchFullName },
                { "merchant_short_name", mchApply.MchShortName ?? mchApply.MchFullName },
                { "merchant_type", mchApply.MerchantType == 1 ? "1" : "2" },
                { "prov_code", mchApply.ProvinceCode ?? "" },
                { "city_code", mchApply.CityCode ?? "" },
                { "area_code", mchApply.DistrictCode ?? "" },
                { "address", mchApply.Address ?? "" },
                { "notify_url", GetNotifyUrl() },
            };

            reqData["contact_info"] = new JObject
            {
                { "name", mchApply.ContactName ?? "" },
                { "mobile_no", mchApply.ContactPhone ?? "" },
                { "email", mchApply.ContactEmail ?? "" },
            };

            TryApplyDetailFields(mchApply, reqData);

            return reqData;
        }

        private static void TryApplyDetailFields(MchApplyDto mchApply, JObject reqData)
        {
            if (string.IsNullOrEmpty(mchApply.ApplyDetailInfo)) return;
            try
            {
                var ext = JObject.Parse(mchApply.ApplyDetailInfo);

                if (ext.TryGetValue("business_license", out var licVal) && licVal is JObject licObj)
                {
                    var lic = new JObject();
                    if (licObj.TryGetValue("license_no", out var ln)) lic["license_no"] = ln.ToString();
                    if (licObj.TryGetValue("license_name", out var lname)) lic["license_name"] = lname.ToString();
                    if (licObj.TryGetValue("legal_person", out var lp)) lic["legal_person"] = lp.ToString();
                    if (lic.Properties().Any()) reqData["business_license"] = lic;
                }
                else
                {
                    var lic = new JObject();
                    if (ext.TryGetValue("licence_no", out var ln)) lic["license_no"] = ln.ToString();
                    if (ext.TryGetValue("legal_person", out var lp)) lic["legal_person"] = lp.ToString();
                    if (lic.Properties().Any()) reqData["business_license"] = lic;
                }

                if (ext.TryGetValue("bank_card_info", out var bankVal) && bankVal is JObject bankObj)
                {
                    var bank = new JObject();
                    if (bankObj.TryGetValue("card_no", out var cn)) bank["card_no"] = cn.ToString();
                    if (bankObj.TryGetValue("card_name", out var cname)) bank["card_name"] = cname.ToString();
                    if (bankObj.TryGetValue("bank_name", out var bname)) bank["bank_name"] = bname.ToString();
                    if (bankObj.TryGetValue("bank_branch", out var bb)) bank["bank_branch"] = bb.ToString();
                    if (bank.Properties().Any()) reqData["bank_card_info"] = bank;
                }
                else
                {
                    var bank = new JObject();
                    if (ext.TryGetValue("bank_account_no", out var ban)) bank["card_no"] = ban.ToString();
                    if (ext.TryGetValue("bank_account_name", out var bn)) bank["card_name"] = bn.ToString();
                    if (ext.TryGetValue("bank_name", out var bk)) bank["bank_name"] = bk.ToString();
                    if (bank.Properties().Any()) reqData["bank_card_info"] = bank;
                }

                if (ext.TryGetValue("contact_cert_no", out var certNo))
                {
                    if (reqData["contact_info"] is JObject ci)
                        ci["cert_no"] = certNo.ToString();
                }
            }
            catch
            {
            }
        }

        #endregion

        #region 状态映射

        private static string MapAuditState(string auditState)
        {
            return auditState?.ToUpper() switch
            {
                "APPROVED" or "SUCCESS" or "PASS" => "SUCCESS",
                "REJECTED" or "FAIL" => "FAIL",
                "PENDING" or "AUDITING" or "PROCESSING" => "ING",
                _ => "ING"
            };
        }

        #endregion
    }
}
