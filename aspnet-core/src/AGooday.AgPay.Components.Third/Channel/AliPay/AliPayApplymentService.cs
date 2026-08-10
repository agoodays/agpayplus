using System.Text.Json;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Applyment;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Application.DataTransfer;
using Aop.Api.Domain;
using Aop.Api.Request;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 支付宝-特约商户进件服务
    /// 使用 ant.merchant.expand.indirect.* 系列 API（服务商模式）
    /// </summary>
    public class AliPayApplymentService : AbstractApplymentService
    {
        public AliPayApplymentService(
            ILogger<AliPayApplymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode() => CS.IF_CODE.ALIPAY;

        public override Task<ApplymentSubmitRS> SubmitAsync(ApplymentSubmitRQ rq, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentSubmitRS();

            var wrapper = isvCtx?.AlipayClientWrapper;
            if (wrapper == null)
            {
                result.ErrMsg = "支付宝客户端不可用";
                return Task.FromResult(result);
            }

            if (string.IsNullOrEmpty(mchApply?.MchFullName))
            {
                result.ErrMsg = "缺少必要参数：商户全称";
                return Task.FromResult(result);
            }

            var request = new AntMerchantExpandIndirectCreateRequest();
            var model = BuildCreateModel(mchApply);
            request.SetBizModel(model);
            result.ChannelReqMsg = JsonSerializer.Serialize(model);

            try
            {
                var response = wrapper.Execute(request);
                result.ChannelResp = JsonSerializer.Serialize(response);

                if (!response.IsError)
                {
                    result.ChannelMchId = response.SubMerchantId;
                    result.ChannelApplyNo = response.SubMerchantId;
                    result.ChannelState = 1;
                }
                else
                {
                    result.ErrMsg = AliPayKit.AppendErrMsg(response.Msg, response.SubMsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付宝特约商户进件提交异常");
                result.ErrMsg = $"支付宝进件异常: {ex.Message}";
            }

            return Task.FromResult(result);
        }

        public override Task<ApplymentQueryRS> QueryAsync(string applyId, string channelApplyNo, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentQueryRS { ChannelState = "FAIL" };

            var wrapper = isvCtx?.AlipayClientWrapper;
            if (wrapper == null)
            {
                result.ChannelStateDesc = "支付宝客户端不可用";
                return Task.FromResult(result);
            }

            try
            {
                var request = new AntMerchantExpandIndirectQueryRequest();
                var model = new AntMerchantExpandIndirectQueryModel();

                if (!string.IsNullOrEmpty(channelMchId))
                    model.SubMerchantId = channelMchId;
                else if (!string.IsNullOrEmpty(applyId))
                    model.ExternalId = applyId;

                request.SetBizModel(model);

                var response = wrapper.Execute(request);
                result.ChannelResp = JsonSerializer.Serialize(response);

                if (!response.IsError)
                {
                    result.ChannelMchId = response.SubMerchantId;
                    result.ChannelState = "SUCCESS";
                    result.ChannelStateDesc = $"商户查询成功，名称: {response.Name ?? response.AliasName}";
                }
                else
                {
                    result.ChannelStateDesc = AliPayKit.AppendErrMsg(response.Msg, response.SubMsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付宝特约商户进件查询异常");
                result.ChannelStateDesc = $"查询异常: {ex.Message}";
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// 支付宝普通进件模式（ant.merchant.expand.indirect.create）不生成签约链接
        /// 签约是进件后商户在支付宝商户平台自助完成
        /// </summary>
        public override Task<ApplymentSubmitRS> GetSignUrlAsync(string applyId, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            return Task.FromResult(new ApplymentSubmitRS
            {
                ErrMsg = "支付宝普通进件模式不支持获取签约链接，请使用 ZFT（代扣）模式或商户自助签约"
            });
        }

        /// <summary>
        /// 支付宝服务商模式由系统自动完成银行账户验证，无需小额打款
        /// </summary>
        public override Task<ApplymentQueryRS> VerifyAsync(string applyId, long verifyAmount, string verifyCode, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            return Task.FromResult(new ApplymentQueryRS
            {
                ChannelState = "SUCCESS",
                ChannelStateDesc = "支付宝服务商模式无需小额打款验证"
            });
        }

        #region 构建请求体

        private static AntMerchantExpandIndirectCreateModel BuildCreateModel(MchApplyDto mchApply)
        {
            var model = new AntMerchantExpandIndirectCreateModel
            {
                ExternalId = mchApply.ApplyId,
                Name = mchApply.MchFullName,
                AliasName = mchApply.MchShortName ?? mchApply.MchFullName,
                ServicePhone = mchApply.ContactPhone,
                ServiceCodes = new List<string> { "FaceToFace", "Wap", "App" },
                PayCodeInfo = new List<string> { "FaceToFace", "Wap" },
                Source = "ISV",
            };

            var ext = TryGetDetail(mchApply);
            if (ext.HasValue)
            {
                var root = ext.Value;
                if (root.TryGetProperty("business_license", out var bl))
                    model.BusinessLicense = bl.GetString();
                if (root.TryGetProperty("business_license_type", out var blt))
                    model.BusinessLicenseType = blt.GetString();
                if (root.TryGetProperty("mcc", out var mcc))
                    model.Mcc = mcc.GetString();
                if (root.TryGetProperty("memo", out var memo))
                    model.Memo = memo.GetString();
                if (root.TryGetProperty("category_id", out var cat))
                    model.CategoryId = cat.GetString();
            }

            model.AddressInfo = BuildAddressInfo(mchApply);
            model.ContactInfo = BuildContactInfo(mchApply);
            model.BankcardInfo = BuildBankCardInfo(mchApply, ext);
            model.SiteInfo = BuildSiteInfo(mchApply, ext);

            return model;
        }

        private static List<AddressInfo> BuildAddressInfo(MchApplyDto mchApply)
        {
            var list = new List<AddressInfo>
            {
                new AddressInfo
                {
                    Type = "01",
                    ProvinceCode = mchApply.ProvinceCode,
                    CityCode = mchApply.CityCode,
                    DistrictCode = mchApply.DistrictCode,
                    Address = mchApply.Address,
                }
            };
            return list;
        }

        private static List<ContactInfo> BuildContactInfo(MchApplyDto mchApply)
        {
            var list = new List<ContactInfo>
            {
                new ContactInfo
                {
                    Type = "M",
                    Name = mchApply.ContactName,
                    Mobile = mchApply.ContactPhone,
                    Email = mchApply.ContactEmail,
                }
            };

            var ext = TryGetDetail(mchApply);
            if (ext.HasValue)
            {
                var root = ext.Value;
                if (root.TryGetProperty("contact_cert_no", out var cn))
                    list[0].IdCardNo = cn.GetString();
            }

            return list;
        }

        private static List<BankCardInfo> BuildBankCardInfo(MchApplyDto mchApply, JsonElement? ext)
        {
            var list = new List<BankCardInfo>();
            if (!ext.HasValue) return list;

            var root = ext.Value;
            if (root.TryGetProperty("bank_account_no", out var ban))
            {
                list.Add(new BankCardInfo
                {
                    CardNo = ban.GetString(),
                    CardName = root.TryGetProperty("bank_account_name", out var bn) ? bn.GetString() : mchApply.MchFullName,
                    BankBranchName = root.TryGetProperty("bank_branch", out var br) ? br.GetString() : null,
                });
            }

            return list;
        }

        private static List<SiteInfo> BuildSiteInfo(MchApplyDto mchApply, JsonElement? ext)
        {
            var list = new List<SiteInfo>();
            if (!ext.HasValue) return list;

            var root = ext.Value;

            if (root.TryGetProperty("site_url", out var su))
            {
                list.Add(new SiteInfo
                {
                    SiteType = "01",
                    SiteUrl = su.GetString(),
                    SiteName = root.TryGetProperty("site_name", out var sn) ? sn.GetString() : mchApply.MchShortName,
                });
            }

            if (root.TryGetProperty("mini_app_id", out var ma))
            {
                list.Add(new SiteInfo
                {
                    SiteType = "03",
                    TinyAppId = ma.GetString(),
                    SiteName = root.TryGetProperty("mini_app_name", out var mn) ? mn.GetString() : mchApply.MchShortName,
                });
            }

            return list;
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
    }
}
