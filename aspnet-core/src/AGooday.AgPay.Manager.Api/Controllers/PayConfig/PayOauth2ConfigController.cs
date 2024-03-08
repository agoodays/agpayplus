using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.PayConfig
{
    [Route("api/payOauth2Config")]
    [ApiController, Authorize]
    public class PayOauth2ConfigController : CommonController
    {
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IAgentInfoService _agentInfoService;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public PayOauth2ConfigController(ILogger<PayOauth2ConfigController> logger,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            IAgentInfoService agentInfoService,
            IPayInterfaceConfigService payIfConfigService, 
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
            _agentInfoService = agentInfoService;
            _payIfConfigService = payIfConfigService;
        }

        /// <summary>
        /// 查询配置条目
        /// </summary>
        /// <param name="infoId"></param>
        /// <param name="configMode"></param>
        /// <returns></returns>
        [HttpGet, Route("diyList"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_OAUTH2_CONFIG_VIEW, PermCode.MGR.ENT_AGENT_PAY_CONFIG_VIEW)]
        public ApiRes List(string configMode, string infoId)
        {
            List<object> result = new List<object>();
            if (configMode.Equals(CS.CONFIG_MODE.MGR_AGENT) || configMode.Equals(CS.CONFIG_MODE.AGENT_SUBAGENT))
            {
                var agentInfo = _agentInfoService.GetById(infoId);
                infoId = agentInfo.IsvNo;
                result.Add(new { InfoId = infoId, Remark = "服务商默认" });
            }
            string infoType = GetInfoType(configMode);
            var data = _payIfConfigService.GetPayOauth2ConfigByInfoId(infoType, infoId)
                .GroupBy(g => new { g.InfoId, g.Remark })
                .Select(s => new { s.Key.InfoId, s.Key.Remark });
            result.AddRange(data);
            return ApiRes.Ok(result);
        }

        [HttpPost, Route("diyList"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_OAUTH2_CONFIG_ADD)]
        public ApiRes Save(PayOauth2ConfigRequest model)
        {
            string infoType = GetInfoType(model.ConfigMode);
            var data = _payIfConfigService.GetByInfoId(infoType, model.CopySourceInfoId);
            data ??= new List<PayInterfaceConfigDto>()
            {
                new PayInterfaceConfigDto() { InfoType = infoType, InfoId = model.InfoId, IfCode = CS.IF_CODE.WXPAY, Remark = model.Remark },
            };
            var infoId = $"{model.InfoId}_LIST_{DateTime.Now:yyyyMMddHHmmss}";
            //添加更新者信息
            long userId = GetCurrentUser().SysUser.SysUserId;
            string realName = GetCurrentUser().SysUser.Realname;
            foreach (var item in data)
            {
                item.Id = null;
                item.InfoId = infoId;
                item.Remark = model.Remark;
                item.CreatedUid = userId;
                item.CreatedBy = realName;
                item.CreatedAt = DateTime.Now;
                item.UpdatedUid = userId;
                item.UpdatedBy = realName;
                item.UpdatedAt = DateTime.Now;
                _payIfConfigService.Add(item);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 获取Oauth2参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("savedConfigs"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_OAUTH2_CONFIG_VIEW, PermCode.MGR.ENT_MCH_OAUTH2_CONFIG_VIEW)]
        public ApiRes GetByInfoId(string configMode, string infoId, string ifCode)
        {
            string infoType = GetInfoType(configMode);
            var payInterfaceConfig = _payIfConfigService.GetByInfoIdAndIfCode(infoType, infoId, ifCode);
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
                    var mchApp = _mchAppService.GetById(infoId);
                    var mchInfo = _mchInfoService.GetById(mchApp.MchNo);
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
        [PermissionAuth(PermCode.MGR.ENT_ISV_OAUTH2_CONFIG_ADD, PermCode.MGR.ENT_MCH_OAUTH2_CONFIG_ADD)]
        public ApiRes SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            //添加更新者信息
            long userId = GetCurrentUser().SysUser.SysUserId;
            string realName = GetCurrentUser().SysUser.Realname;
            dto.UpdatedUid = userId;
            dto.UpdatedBy = realName;
            dto.UpdatedAt = DateTime.Now;

            //根据 服务商号、接口类型 获取商户参数配置
            var dbRecoed = _payIfConfigService.GetByInfoIdAndIfCode(dto.InfoType, dto.InfoId, dto.IfCode);
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
            var result = _payIfConfigService.SaveOrUpdate(dto);
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
                case CS.CONFIG_MODE.MGR_AGENT:
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
