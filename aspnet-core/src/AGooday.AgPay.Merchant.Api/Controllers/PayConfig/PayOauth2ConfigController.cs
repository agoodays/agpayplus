using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayConfig
{
    [Route("api/payOauth2Config")]
    [ApiController, Authorize]
    public class PayOauth2ConfigController : CommonController
    {
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public PayOauth2ConfigController(ILogger<PayOauth2ConfigController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            IPayInterfaceConfigService payIfConfigService)
            : base(logger, cacheService, authService)
        {
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
            _payIfConfigService = payIfConfigService;
        }

        /// <summary>
        /// 获取Oauth2参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("savedConfigs"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_MCH_OAUTH2_CONFIG_VIEW)]
        public async Task<ApiRes> GetByInfoIdAsync(string configMode, string infoId, string ifCode)
        {
            string infoType = GetInfoType(configMode);
            var payInterfaceConfig = await _payIfConfigService.GetByInfoIdAndIfCodeAsync(infoType, infoId, ifCode);
            payInterfaceConfig ??= new PayInterfaceConfigDto()
            {
                InfoType = infoType,
                InfoId = infoId,
                IfCode = ifCode,
                State = CS.YES
            };
            switch (infoType)
            {
                case CS.INFO_TYPE.ISV_OAUTH2:
                    // 敏感数据脱敏
                    if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                    {
                        var isvParams = IsvOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
                        if (isvParams != null)
                        {
                            payInterfaceConfig.IfParams = isvParams.DeSenData();
                        }
                    }
                    break;
                case CS.INFO_TYPE.MCH_APP_OAUTH2:
                    var mchApp = await _mchAppService.GetByIdAsync(infoId);
                    var mchInfo = await _mchInfoService.GetByIdAsync(mchApp.MchNo);
                    // 敏感数据脱敏
                    if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                    {
                        // 普通商户的支付参数执行数据脱敏
                        if (mchInfo.Type == CS.MCH_TYPE_NORMAL)
                        {
                            var mchParams = NormalMchOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
                            if (mchParams != null)
                            {
                                payInterfaceConfig.IfParams = mchParams.DeSenData();
                            }
                        }
                        if (mchInfo.Type == CS.MCH_TYPE_ISVSUB)
                        {
                            var mchParams = IsvSubMchOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
                            if (mchParams != null)
                            {
                                payInterfaceConfig.IfParams = mchParams.DeSenData();
                            }
                        }
                    }
                    break;
            }
            return ApiRes.Ok(payInterfaceConfig);
        }

        /// <summary>
        /// 支付接口Oauth2参数配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("configParams"), MethodLog("更新Oauth2配置")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_OAUTH2_CONFIG_ADD)]
        public async Task<ApiRes> SaveOrUpdateAsync(PayInterfaceConfigDto dto)
        {
            //添加更新者信息
            long userId = await GetCurrentUserIdAsync();
            string realName = (await GetCurrentUserAsync()).SysUser.Realname;
            dto.UpdatedUid = userId;
            dto.UpdatedBy = realName;
            dto.UpdatedAt = DateTime.Now;

            //根据 服务商号、接口类型 获取商户参数配置
            var dbRecoed = await _payIfConfigService.GetByInfoIdAndIfCodeAsync(dto.InfoType, dto.InfoId, dto.IfCode);
            //若配置存在，为saveOrUpdate添加ID，第一次配置添加创建者
            if (dbRecoed != null)
            {
                dto.Id = dbRecoed.Id;
                dto.CreatedUid = dbRecoed.CreatedUid;
                dto.CreatedBy = dbRecoed.CreatedBy;
                dto.CreatedAt = dbRecoed.CreatedAt;
                // 合并支付参数
                dto.IfParams = StringUtil.Merge(dbRecoed.IfParams, dto.IfParams);
            }
            else
            {
                dto.CreatedUid = userId;
                dto.CreatedBy = realName;
                dto.CreatedAt = DateTime.Now;
            }
            var result = await _payIfConfigService.SaveOrUpdateAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "配置失败");
            }

            return ApiRes.Ok();
        }

        private static string GetInfoType(string configMode)
        {
            var infoType = string.Empty;
            switch (configMode)
            {
                case CS.CONFIG_MODE.MGR_ISV:
                    infoType = CS.INFO_TYPE.ISV_OAUTH2;
                    break;
                case CS.CONFIG_MODE.MGR_MCH:
                case CS.CONFIG_MODE.MCH_SELF_APP1:
                case CS.CONFIG_MODE.MCH_SELF_APP2:
                    infoType = CS.INFO_TYPE.MCH_APP_OAUTH2;
                    break;
                default:
                    break;
            }

            return infoType;
        }
    }
}
