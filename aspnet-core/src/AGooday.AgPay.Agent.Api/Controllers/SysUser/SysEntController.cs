using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Agent.Api.Controllers.SysUser
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
            ISysEntitlementService sysEntService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _sysEntService = sysEntService;
        }

        /// <summary>
        /// 查询权限集合
        /// </summary>
        /// <param name="sysType"></param>
        /// <returns></returns>
        [HttpGet, Route("showTree"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_UR_ROLE_DIST)]
        public ApiRes ShowTree()
        {
            //查询全部数据
            var sysEnt = _sysEntService.GetBySysType(CS.SYS_TYPE.AGENT, null);

            //递归转换为树状结构
            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};
            var jsonArray = JArray.FromObject(sysEnt);
            var leftMenuTree = new TreeDataBuilder(jsonArray, "entId", "pid", "children", "entSort", true).BuildTreeObject();
            return ApiRes.Ok(leftMenuTree);
        }
    }
}
