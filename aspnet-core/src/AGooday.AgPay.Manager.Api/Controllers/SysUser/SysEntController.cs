using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using AGooday.AgPay.Manager.Api.Extensions;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authorization;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Manager.Api.Authorization;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    /// <summary>
    /// 权限 菜单 管理
    /// </summary>
    [Route("/api/sysEnts")]
    [ApiController, Authorize]
    public class SysEntController : CommonController
    {
        private readonly ILogger<SysEntController> _logger;
        private readonly ISysEntitlementService _sysEntService;

        public SysEntController(ILogger<SysEntController> logger, RedisUtil client,
            ISysUserService sysUserService,
            ISysEntitlementService sysEntService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _sysEntService = sysEntService;
        }

        /// <summary>
        /// 查看资源权限
        /// </summary>
        /// <param name="sysType"></param>
        /// <param name="entId"></param>
        /// <returns></returns>
        [HttpGet, Route("bySysType")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_LIST)]
        public ApiRes BySystem(string sysType, string entId)
        {
            var sysEnts = _sysEntService.GetBySysType(sysType, entId).FirstOrDefault();
            return ApiRes.Ok(sysEnts);
        }

        /// <summary>
        /// 更新资源权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{entId}")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_EDIT)]
        public ApiRes Update(string entId, SysEntModifyDto dto)
        {
            _sysEntService.Update(dto);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查询权限集合
        /// </summary>
        /// <param name="sysType"></param>
        /// <returns></returns>
        [HttpGet, Route("showTree")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ENT_LIST, PermCode.MGR.ENT_UR_ROLE_ENT_EDIT)]
        //public ActionResult ShowTree(string sysType)
        public ApiRes ShowTree(string sysType)
        {
            //查询全部数据
            var sysEnt = _sysEntService.GetBySysType(sysType, null);

            //递归转换为树状结构
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var jsonArray = JArray.FromObject(sysEnt);
            var leftMenuTree = new TreeDataBuilder(jsonArray, "entId", "pid", "children", "entSort", true).BuildTreeObject();
            //var json = JsonConvert.SerializeObject(ApiRes.Ok(leftMenuTree));
            //return Content(json, "application/json");
            return ApiRes.Ok(leftMenuTree);
        }
    }
}
