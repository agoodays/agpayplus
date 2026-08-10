using System.Text.Json;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Applyment;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Application.DataTransfer;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;

namespace AGooday.AgPay.Components.Third.Channel.WxPay
{
    /// <summary>
    /// 微信支付-特约商户进件服务
    /// 服务商模式下的 sub_mch 进件（V3 API）
    /// </summary>
    public class WxPayApplymentService : AbstractApplymentService
    {
        public WxPayApplymentService(
            ILogger<WxPayApplymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode() => CS.IF_CODE.WXPAY;

        public override async Task<ApplymentSubmitRS> SubmitAsync(ApplymentSubmitRQ rq, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentSubmitRS();

            var client = isvCtx?.WxServiceWrapper?.Client as WechatTenpayClient;
            if (client == null)
            {
                result.ErrMsg = "微信支付V3客户端不可用";
                return result;
            }

            if (string.IsNullOrEmpty(mchApply?.MchFullName))
            {
                result.ErrMsg = "缺少必要参数：商户全称";
                return result;
            }

            var request = BuildCreateRequest(mchApply);
            result.ChannelReqMsg = JsonSerializer.Serialize(request);

            try
            {
                var response = await client.ExecuteCreateApplyForSubMerchantApplymentAsync(request);
                result.ChannelResp = JsonSerializer.Serialize(response);

                if (response.IsSuccessful())
                {
                    result.ChannelApplyNo = response.ApplymentId.ToString();
                    result.ChannelState = 1;

                    var queryResp = await QueryByBusinessCode(client, mchApply.ApplyId);
                    if (queryResp != null && queryResp.IsSuccessful())
                    {
                        result.ChannelApplyNo = queryResp.ApplymentId.ToString();
                        MapQueryResultToSubmit(result, queryResp);
                    }
                }
                else
                {
                    result.ErrMsg = response.ErrorMessage ?? response.ErrorCode ?? "微信进件失败";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "微信特约商户进件提交异常");
                result.ErrMsg = $"微信进件异常: {ex.Message}";
            }

            return result;
        }

        public override async Task<ApplymentQueryRS> QueryAsync(string applyId, string channelApplyNo, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentQueryRS { ChannelState = "FAIL" };

            var client = isvCtx?.WxServiceWrapper?.Client as WechatTenpayClient;
            if (client == null)
            {
                result.ChannelStateDesc = "微信支付V3客户端不可用";
                return result;
            }

            try
            {
                WechatTenpayResponse queryResp = null;

                if (!string.IsNullOrEmpty(channelApplyNo) && long.TryParse(channelApplyNo, out long applymentId))
                {
                    var req = new GetApplyForSubMerchantApplymentByApplymentIdRequest { ApplymentId = applymentId };
                    queryResp = await client.ExecuteGetApplyForSubMerchantApplymentByApplymentIdAsync(req);
                }
                else
                {
                    queryResp = await QueryByBusinessCode(client, applyId);
                }

                result.ChannelResp = JsonSerializer.Serialize(queryResp);

                if (queryResp != null && queryResp.IsSuccessful())
                {
                    if (queryResp is GetApplyForSubMerchantApplymentByApplymentIdResponse byId)
                        FillQueryResult(result, byId);
                    else if (queryResp is GetApplyForSubMerchantApplymentByBusinessCodeResponse byBiz)
                        FillQueryResult(result, byBiz);
                }
                else
                {
                    result.ChannelStateDesc = queryResp?.ErrorMessage ?? queryResp?.ErrorCode ?? "查询失败";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "微信特约商户进件查询异常");
                result.ChannelStateDesc = $"查询异常: {ex.Message}";
            }

            return result;
        }

        public override async Task<ApplymentSubmitRS> GetSignUrlAsync(string applyId, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentSubmitRS();

            var client = isvCtx?.WxServiceWrapper?.Client as WechatTenpayClient;
            if (client == null)
            {
                result.ErrMsg = "微信支付V3客户端不可用";
                return result;
            }

            try
            {
                var queryResp = await QueryByBusinessCode(client, applyId);
                if (queryResp != null && queryResp.IsSuccessful())
                {
                    if (!string.IsNullOrEmpty(queryResp.SignUrl))
                    {
                        result.SignUrl = queryResp.SignUrl;
                        result.ChannelApplyNo = queryResp.ApplymentId.ToString();
                        result.ChannelMchId = queryResp.SubMerchantId;
                        result.ChannelState = 3;
                    }
                    else
                    {
                        result.ErrMsg = "暂无可获取的签约链接，请等待微信审核或检查进件状态";
                    }
                }
                else
                {
                    result.ErrMsg = queryResp?.ErrorMessage ?? "查询失败";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取微信签约链接异常");
                result.ErrMsg = $"获取签约链接异常: {ex.Message}";
            }

            return result;
        }

        public override Task<ApplymentQueryRS> VerifyAsync(string applyId, long verifyAmount, string verifyCode, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            return Task.FromResult(new ApplymentQueryRS
            {
                ChannelState = "SUCCESS",
                ChannelStateDesc = "微信服务商模式无需小额打款验证"
            });
        }

        #region 构建请求体

        private static CreateApplyForSubMerchantApplymentRequest BuildCreateRequest(MchApplyDto mchApply)
        {
            var req = new CreateApplyForSubMerchantApplymentRequest
            {
                BusinessCode = mchApply.ApplyId,
                Contact = BuildContact(mchApply),
                Subject = BuildSubject(mchApply),
                Business = BuildBusiness(mchApply),
                BankAccount = BuildBankAccount(mchApply),
            };

            if (!string.IsNullOrEmpty(mchApply.ApplyParams))
            {
                try
                {
                    using var ext = JsonDocument.Parse(mchApply.ApplyParams);
                    req.Settlement = BuildSettlement(ext);
                    req.Addition = BuildAddition(ext);
                }
                catch { }
            }

            return req;
        }

        private static CreateApplyForSubMerchantApplymentRequest.Types.Contact BuildContact(MchApplyDto mchApply)
        {
            var contact = new CreateApplyForSubMerchantApplymentRequest.Types.Contact
            {
                ContactType = "LEGAL",
                ContactName = mchApply.ContactName,
                MobileNumber = EncryptField(mchApply.ContactPhone),
                Email = mchApply.ContactEmail,
            };

            var ext = TryGetDetail(mchApply);
            if (ext.HasValue)
            {
                var root = ext.Value;
                if (root.TryGetProperty("contact_cert_type", out var ct))
                    contact.IdentityType = ct.GetString();
                if (root.TryGetProperty("contact_cert_no", out var cn))
                    contact.IdNumber = EncryptField(cn.GetString());
            }

            return contact;
        }

        private static CreateApplyForSubMerchantApplymentRequest.Types.Subject BuildSubject(MchApplyDto mchApply)
        {
            var subject = new CreateApplyForSubMerchantApplymentRequest.Types.Subject();

            subject.SubjectType = mchApply.MerchantType switch
            {
                1 => "SUBJECT_TYPE_INDIVIDUAL",
                _ => "SUBJECT_TYPE_ENTERPRISE"
            };

            var ext = TryGetDetail(mchApply);
            if (ext.HasValue)
            {
                var root = ext.Value;
                var lic = new CreateApplyForSubMerchantApplymentRequest.Types.Subject.Types.BusinessLicense
                {
                    MerchantName = mchApply.MchFullName,
                };

                if (root.TryGetProperty("licence_no", out var ln))
                    lic.LicenceNumber = ln.GetString();
                if (root.TryGetProperty("legal_person", out var lp))
                    lic.LegalPerson = lp.GetString();
                if (!string.IsNullOrEmpty(mchApply.Address))
                    lic.Address = mchApply.Address;
                if (root.TryGetProperty("licence_begin_date", out var bd))
                    lic.PeriodBeginDateString = bd.GetString();
                if (root.TryGetProperty("licence_end_date", out var ed))
                    lic.PeriodEndDateString = ed.GetString();

                if (!string.IsNullOrEmpty(lic.LicenceNumber))
                    subject.BusinessLicense = lic;
            }

            return subject;
        }

        private static CreateApplyForSubMerchantApplymentRequest.Types.Business BuildBusiness(MchApplyDto mchApply)
        {
            return new CreateApplyForSubMerchantApplymentRequest.Types.Business
            {
                MerchantShortName = mchApply.MchShortName ?? mchApply.MchFullName,
                ServicePhone = mchApply.ContactPhone,
            };
        }

        private static CreateApplyForSubMerchantApplymentRequest.Types.BankAccount BuildBankAccount(MchApplyDto mchApply)
        {
            var account = new CreateApplyForSubMerchantApplymentRequest.Types.BankAccount
            {
                AccountType = "BANK_ACCOUNT_TYPE_CORPORATE",
            };

            var ext = TryGetDetail(mchApply);
            if (ext.HasValue)
            {
                var root = ext.Value;
                if (root.TryGetProperty("bank_account_name", out var bn))
                    account.AccountName = bn.GetString();
                if (root.TryGetProperty("bank_account_no", out var ban))
                    account.AccountNumber = EncryptField(ban.GetString());
                if (root.TryGetProperty("bank_name", out var bk))
                    account.AccountBank = bk.GetString();
                if (root.TryGetProperty("bank_branch", out var br))
                    account.BankBranchName = br.GetString();
            }

            if (string.IsNullOrEmpty(account.AccountName))
                account.AccountName = mchApply.MchFullName;

            return account;
        }

        private static CreateApplyForSubMerchantApplymentRequest.Types.Settlement BuildSettlement(JsonDocument ext)
        {
            var settlement = new CreateApplyForSubMerchantApplymentRequest.Types.Settlement();
            if (ext.RootElement.TryGetProperty("settlement", out var st))
            {
                if (st.TryGetProperty("settlement_id", out var sid) && int.TryParse(sid.GetString(), out var id))
                    settlement.SettlementId = id;
                if (st.TryGetProperty("qualification_type", out var qt))
                    settlement.QualificationType = qt.GetString();
                if (st.TryGetProperty("activity_id", out var aid))
                    settlement.ActivityId = aid.GetString();
            }
            return settlement;
        }

        private static CreateApplyForSubMerchantApplymentRequest.Types.Addition BuildAddition(JsonDocument ext)
        {
            var addition = new CreateApplyForSubMerchantApplymentRequest.Types.Addition();
            if (ext.RootElement.TryGetProperty("addition", out var ad))
            {
                if (ad.TryGetProperty("business_message", out var bm))
                    addition.BusinessAdditionMessage = bm.GetString();
            }
            return addition;
        }

        private static JsonElement? TryGetDetail(MchApplyDto mchApply)
        {
            if (string.IsNullOrEmpty(mchApply.ApplyDetailInfo)) return null;
            try
            {
                using var doc = JsonDocument.Parse(mchApply.ApplyDetailInfo);
                return doc.RootElement.Clone();
            }
            catch { return null; }
        }

        #endregion

        #region 查询辅助

        private static async Task<GetApplyForSubMerchantApplymentByBusinessCodeResponse> QueryByBusinessCode(
            WechatTenpayClient client, string businessCode)
        {
            var req = new GetApplyForSubMerchantApplymentByBusinessCodeRequest { BusinessCode = businessCode };
            return await client.ExecuteGetApplyForSubMerchantApplymentByBusinessCodeAsync(req);
        }

        private static void FillQueryResult(ApplymentQueryRS result, GetApplyForSubMerchantApplymentByApplymentIdResponse resp)
        {
            FillCommonQueryResult(result, resp.ApplymentState, resp.ApplymentStateMessage,
                resp.SubMerchantId, resp.SignUrl);
        }

        private static void FillQueryResult(ApplymentQueryRS result, GetApplyForSubMerchantApplymentByBusinessCodeResponse resp)
        {
            FillCommonQueryResult(result, resp.ApplymentState, resp.ApplymentStateMessage,
                resp.SubMerchantId, resp.SignUrl);
        }

        private static void FillCommonQueryResult(ApplymentQueryRS result, string state, string stateMsg,
            string subMchId, string signUrl)
        {
            result.ChannelState = MapWxState(state);
            result.ChannelStateDesc = stateMsg;
            result.ChannelMchId = subMchId;
            result.SignUrl = signUrl;
        }

        private static void MapQueryResultToSubmit(ApplymentSubmitRS result, GetApplyForSubMerchantApplymentByBusinessCodeResponse resp)
        {
            var state = MapWxState(resp.ApplymentState);
            result.ChannelMchId = resp.SubMerchantId;
            result.SignUrl = resp.SignUrl;

            result.ChannelState = state switch
            {
                "SUCCESS" => 1,
                "SIGNING" => 3,
                "VERIFYING" => 4,
                _ => 1
            };
        }

        #endregion

        #region 状态映射

        private static string MapWxState(string wxState)
        {
            return wxState switch
            {
                "FINISHED" => "SUCCESS",
                "REJECTED" => "FAIL",
                "EXECUTE_FAILED" => "FAIL",
                "TO_BE_SIGNED" => "SIGNING",
                "TO_BE_VERIFIED" => "VERIFYING",
                _ => "ING"
            };
        }

        #endregion

        #region 敏感字段加密

        /// <summary>
        /// 微信支付要求敏感字段（身份证号、手机号、银行账号等）使用 AES-256-GCM 加密
        /// 加密密钥 = 微信支付 APIv3Key（32字节）
        /// TODO: 生产环境必须实现
        /// </summary>
        private static string EncryptField(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return null;
            return plainText;
        }

        #endregion
    }
}
