using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    /// <summary>
    /// 权限 菜单 管理
    /// </summary>
    [Route("api/sysEnts")]
    [ApiController, Authorize]
    public class SysEntController : CommonController
    {
        private readonly ISysEntitlementService _sysEntService;

        public SysEntController(ILogger<SysEntController> logger,
            ICacheService cacheService,
            IAuthService authService,
            ISysEntitlementService sysEntService)
            : base(logger, cacheService, authService)
        {
            _sysEntService = sysEntService;
        }

        /// <summary>
        /// 查看资源权限
        /// </summary>
        /// <param name="sysType"></param>
        /// <param name="entId"></param>
        /// <returns></returns>
        [HttpGet, Route("bySysType"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_LIST)]
        public async Task<ApiRes> BySystemAsync(string sysType, string entId)
        {
            var sysEnt = await _sysEntService.GetByKeyAsync(entId, sysType);
            return ApiRes.Ok(sysEnt);
        }

        /// <summary>
        /// 更新资源权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{entId}"), MethodLog("更新资源权限")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_EDIT)]
        public async Task<ApiRes> UpdateAsync(string entId, SysEntitlementDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EntId))
            {
                var sysEnt = await _sysEntService.GetByKeyAsNoTrackingAsync(entId, dto.SysType);
                sysEnt.State = dto.State;
                CopyUtil.CopyProperties(sysEnt, dto);

                dto.UpdatedAt = DateTime.Now;
                await _sysEntService.UpdateAsync(dto);
                return ApiRes.Ok();
            }

            dto.UpdatedAt = DateTime.Now;
            SetMatchRule(dto);

            var updateRecords = new List<SysEntitlementDto>();
            updateRecords.Add(dto);

            var subs = _sysEntService.GetSubSysEntitlementsFromSql(entId, dto.SysType).Where(w => !w.EntId.Equals(entId));
            foreach (var item in subs)
            {
                item.MatchRule = dto.MatchRule;
                item.UpdatedAt = dto.UpdatedAt;
                updateRecords.Add(item);
            }
            var parents = _sysEntService.GetParentSysEntitlementsFromSql(entId, dto.SysType).Where(w => !w.EntId.Equals(entId));
            foreach (var item in parents)
            {
                item.MatchRule ??= new SysEntitlementDto.EntMatchRule();

                var bros = _sysEntService.GetBros(item.SysType, item.Pid, item.EntId).ToList();
                bros.Add(dto);
                bros = bros.Where(a => a.MatchRule != null).ToList();
                if (bros.Any(a => a.MatchRule?.EpUserEnt != null && a.MatchRule.EpUserEnt.HasValue && a.MatchRule.EpUserEnt.Value))
                {
                    item.MatchRule.EpUserEnt = dto.MatchRule.EpUserEnt;
                }
                if (bros.Any(a => a.MatchRule?.UserEntRules != null))
                {
                    item.MatchRule.UserEntRules ??= new List<string>();
                    foreach (var userEntRules in bros.Where(w => w.MatchRule.UserEntRules != null).Select(s => s.MatchRule.UserEntRules))
                    {
                        item.MatchRule.UserEntRules = item.MatchRule.UserEntRules.Union(userEntRules).ToList();
                    }
                }
                if (bros.Any(a => a.MatchRule?.MchSelfCashierEnt != null && a.MatchRule.MchSelfCashierEnt.HasValue && a.MatchRule.MchSelfCashierEnt.Value))
                {
                    item.MatchRule.MchSelfCashierEnt = dto.MatchRule.MchSelfCashierEnt;
                }
                if (bros.Any(a => a.MatchRule?.MchMemberEnt != null && a.MatchRule.MchMemberEnt.HasValue && a.MatchRule.MchMemberEnt.Value))
                {
                    item.MatchRule.MchMemberEnt = dto.MatchRule.MchMemberEnt;
                }
                if (bros.Any(a => a.MatchRule?.MchType != null))
                {
                    item.MatchRule.MchType = dto.MatchRule.MchType;
                }
                if (bros.Any(a => a.MatchRule?.MchLevelArray != null))
                {
                    item.MatchRule.MchLevelArray ??= new List<string>();
                    foreach (var mchLevelArray in bros.Where(w => w.MatchRule.MchLevelArray != null).Select(s => s.MatchRule.MchLevelArray))
                    {
                        item.MatchRule.MchLevelArray = item.MatchRule.MchLevelArray.Union(mchLevelArray).ToList();
                    }
                }
                SetMatchRule(item);
                item.UpdatedAt = dto.UpdatedAt;
                updateRecords.Add(item);
            }

            await _sysEntService.UpdateRangeAsync(updateRecords);
            return ApiRes.Ok();
        }

        private static void SetMatchRule(SysEntitlementDto dto)
        {
            dto.MatchRule ??= new SysEntitlementDto.EntMatchRule();
            dto.MatchRule.EpUserEnt = dto.MatchRule?.EpUserEnt != null && dto.MatchRule.EpUserEnt.HasValue && dto.MatchRule.EpUserEnt.Value ? dto.MatchRule.EpUserEnt : null;
            dto.MatchRule.MchSelfCashierEnt = dto.MatchRule?.MchSelfCashierEnt != null && dto.MatchRule.MchSelfCashierEnt.HasValue && dto.MatchRule.MchSelfCashierEnt.Value ? dto.MatchRule.MchSelfCashierEnt : null;
            dto.MatchRule.MchMemberEnt = dto.MatchRule?.MchMemberEnt != null && dto.MatchRule.MchMemberEnt.HasValue && dto.MatchRule.MchMemberEnt.Value ? dto.MatchRule.MchMemberEnt : null;
            dto.MatchRule.UserEntRules = dto.MatchRule?.UserEntRules != null && dto.MatchRule.UserEntRules?.Count > 0 ? dto.MatchRule.UserEntRules : null;
            dto.MatchRule.MchLevelArray = dto.MatchRule?.MchLevelArray != null && dto.MatchRule.MchLevelArray?.Count > 0 ? dto.MatchRule.MchLevelArray : null;
            dto.MatchRule = dto.MatchRule.GetType()
                .GetProperties().All(property => property.GetValue(dto.MatchRule) == null) ? null : dto.MatchRule;
        }

        /// <summary>
        /// 设置权限匹配规则
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("setMatchRule"), MethodLog("设置权限匹配规则")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_EDIT)]
        public async Task<ApiRes> SetMatchRuleAsync(SysEntMatchRuleSetDto dto)
        {
            var records = _sysEntService.GetByIds(dto.SysType, dto.EntIds);
            foreach (var item in records)
            {
                if (dto.OpType.Equals("add"))
                {
                    item.MatchRule ??= new SysEntitlementDto.EntMatchRule();
                    if (dto.MatchRule?.EpUserEnt != null)
                    {
                        item.MatchRule.EpUserEnt = dto.MatchRule.EpUserEnt;
                    }
                    if (dto.MatchRule?.UserEntRules != null)
                    {
                        item.MatchRule.UserEntRules = item.MatchRule.UserEntRules.Union(dto.MatchRule.UserEntRules).ToList();
                    }
                    if (dto.MatchRule?.MchSelfCashierEnt != null)
                    {
                        item.MatchRule.MchSelfCashierEnt = dto.MatchRule.MchSelfCashierEnt;
                    }
                    if (dto.MatchRule?.MchMemberEnt != null)
                    {
                        item.MatchRule.MchMemberEnt = dto.MatchRule.MchMemberEnt;
                    }
                    if (dto.MatchRule?.MchType != null)
                    {
                        item.MatchRule.MchType = dto.MatchRule.MchType;
                    }
                    if (dto.MatchRule?.MchLevelArray != null)
                    {
                        item.MatchRule.MchLevelArray = item.MatchRule.MchLevelArray.Union(dto.MatchRule.MchLevelArray).ToList();
                    }
                    SetMatchRule(item);
                }

                if (dto.OpType.Equals("delete"))
                {
                    var bros = _sysEntService.GetBros(item.SysType, item.Pid, item.EntId);
                    if (dto.MatchRule?.EpUserEnt != null && item.EntType.Equals(CS.ENT_TYPE.PAGE_OR_BTN))
                    {
                        if (!bros.Any(s => s.MatchRule?.EpUserEnt != null))
                        {
                            item.MatchRule.EpUserEnt = null;
                        }
                    }
                }
                item.UpdatedAt = DateTime.Now;
                await _sysEntService.UpdateAsync(item);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查询权限集合
        /// </summary>
        /// <param name="sysType"></param>
        /// <returns></returns>
        [HttpGet, Route("showTree"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_LIST, PermCode.MGR.ENT_UR_ROLE_ENT_EDIT)]
        //public ActionResult ShowTree(string sysType)
        public ApiRes ShowTree(string sysType)
        {
            //查询全部数据
            var sysEnt = _sysEntService.GetBySysType(sysType, null);

            //递归转换为树状结构
            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};
            var jsonArray = JArray.FromObject(sysEnt);
            var leftMenuTree = new TreeDataBuilder(jsonArray, "entId", "pid", "children", "entSort", true).BuildTreeObject();
            //var json = JsonConvert.SerializeObject(ApiRes.Ok(leftMenuTree));
            //return Content(json, MediaTypeNames.Application.Json);
            return ApiRes.Ok(leftMenuTree);
        }
    }
}
