using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
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
    [Route("/api/sysEnts")]
    [ApiController, Authorize]
    public class SysEntController : CommonController
    {
        private readonly ISysEntitlementService _sysEntService;

        public SysEntController(ILogger<SysEntController> logger,
            ISysEntitlementService sysEntService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
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
        public ApiRes BySystem(string sysType, string entId)
        {
            var sysEnt = _sysEntService.GetByKey(entId, sysType);
            return ApiRes.Ok(sysEnt);
        }

        /// <summary>
        /// 更新资源权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{entId}"), MethodLog("更新资源权限")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_EDIT)]
        public ApiRes Update(string entId, SysEntitlementDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EntId))
            {
                var sysEnt = _sysEntService.GetByKeyAsNoTracking(entId, dto.SysType);
                sysEnt.State = dto.State;
                CopyUtil.CopyProperties(sysEnt, dto);
            }
            dto.UpdatedAt = DateTime.Now;
            _sysEntService.Update(dto);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 设置权限匹配规则
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("setMatchRule"), MethodLog("设置权限匹配规则")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_EDIT)]
        public ApiRes SetMatchRule(SysEntMatchRuleSetDto dto)
        {
            var sysEnts = _sysEntService.GetByIds(dto.SysType, dto.EntIds);
            foreach (var sysEnt in sysEnts)
            {
                if (dto.OpType.Equals("add"))
                {
                    sysEnt.MatchRule = CopyUtil.AssignNonNullProperties(sysEnt.MatchRule, dto.MatchRule);
                }

                if (dto.OpType.Equals("delete"))
                {
                    sysEnt.MatchRule = CopyUtil.RemoveNonNullProperties(sysEnt.MatchRule, dto.MatchRule);
                }
                sysEnt.UpdatedAt = DateTime.Now;
                _sysEntService.Update(sysEnt);
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
            //return Content(json, "application/json");
            return ApiRes.Ok(leftMenuTree);
        }
    }
}
