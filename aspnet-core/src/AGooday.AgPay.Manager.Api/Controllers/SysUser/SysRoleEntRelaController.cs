using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Base.Api.Attributes;
using AGooday.AgPay.Base.Api.Authorization;
using AGooday.AgPay.Base.Api.Controllers;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
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
            ICacheService cacheService,
            IAuthService authService,
            ISysRoleEntRelaService sysRoleEntRelaService)
            : base(logger, cacheService, authService)
        {
            _sysRoleEntRelaService = sysRoleEntRelaService;
        }

        /// <summary>
        /// 角色权限列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ADD, PermCode.MGR.ENT_UR_ROLE_DIST)]
        public async Task<ApiPageRes<SysRoleEntRelaDto>> ListAsync([FromQuery] SysRoleEntRelaQueryDto dto)
        {
            var data = await _sysRoleEntRelaService.GetPaginatedDataAsync(dto);
            return ApiPageRes<SysRoleEntRelaDto>.Pages(data);
        }
    }
}
