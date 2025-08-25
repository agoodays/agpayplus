using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    /// <summary>
    /// 用户角色关联关系
    /// </summary>
    [Route("api/sysUserRoleRelas")]
    [ApiController, Authorize]
    public class SysUserRoleRelaController : CommonController
    {
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysUserRoleRelaController(ILogger<SysUserRoleRelaController> logger,
            ICacheService cacheService,
            IAuthService authService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, cacheService, authService)
        {
            _sysUserRoleRelaService = sysUserRoleRelaService;
        }

        /// <summary>
        /// 用户角色列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_USER_UPD_ROLE)]
        public async Task<ApiPageRes<SysUserRoleRelaDto>> ListAsync([FromQuery] SysUserRoleRelaQueryDto dto)
        {
            var data = await _sysUserRoleRelaService.GetPaginatedDataAsync(dto);
            return ApiPageRes<SysUserRoleRelaDto>.Pages(data);
        }

        /// <summary>
        /// 重置用户角色关联信息
        /// </summary>
        /// <param name="sysUserId"></param>
        /// <returns></returns>
        [HttpPost, Route("relas/{sysUserId}"), MethodLog("重置用户角色关联信息")]
        [PermissionAuth(PermCode.MGR.ENT_UR_USER_UPD_ROLE)]
        public async Task<ApiRes> RelasAsync(long sysUserId, RelasRoleModel model)
        {
            if (model.RoleIds.Count > 0)
            {
                await _sysUserRoleRelaService.SaveUserRoleAsync(sysUserId, model.RoleIds);
                await RefAuthenticationAsync(new List<long> { sysUserId });
            }
            return ApiRes.Ok();
        }
    }
}
