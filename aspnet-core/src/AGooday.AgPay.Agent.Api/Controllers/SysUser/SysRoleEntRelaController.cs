using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers.SysUser
{

    /// <summary>
    /// 角色 权限管理
    /// </summary>
    [Route("api/sysRoleEntRelas")]
    [ApiController, Authorize]
    public class SysRoleEntRelaController : CommonController
    {
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;

        public SysRoleEntRelaController(ILogger<SysRoleEntRelaController> logger,
            ISysRoleEntRelaService sysRoleEntRelaService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _sysRoleEntRelaService = sysRoleEntRelaService;
        }

        /// <summary>
        /// 角色权限列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_UR_ROLE_ADD, PermCode.AGENT.ENT_UR_ROLE_DIST)]
        public ApiPageRes<SysRoleEntRelaDto> List([FromQuery] SysRoleEntRelaQueryDto dto)
        {
            var data = _sysRoleEntRelaService.GetPaginatedData(dto);
            return ApiPageRes<SysRoleEntRelaDto>.Pages(data);
        }
    }
}
