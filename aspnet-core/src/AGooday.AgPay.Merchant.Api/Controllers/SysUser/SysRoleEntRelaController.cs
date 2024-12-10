using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.SysUser
{

    /// <summary>
    /// 角色 权限管理
    /// </summary>
    [Route("api/sysRoleEntRelas")]
    [ApiController, Authorize]
    public class SysRoleEntRelaController : CommonController
    {
        private readonly ISysRoleService _sysRoleService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysRoleEntRelaController(ILogger<SysRoleEntRelaController> logger,
            ISysRoleService sysRoleService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _sysRoleEntRelaService = sysRoleEntRelaService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
            _sysRoleService = sysRoleService;
        }

        /// <summary>
        /// 角色权限列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_DIST), NoLog]
        public async Task<ApiPageRes<SysRoleEntRelaDto>> ListAsync([FromQuery] SysRoleEntRelaQueryDto dto)
        {
            var data = await _sysRoleEntRelaService.GetPaginatedDataAsync(dto);
            return ApiPageRes<SysRoleEntRelaDto>.Pages(data);
        }

        /// <summary>
        /// 重置角色权限关联信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="entIds"></param>
        /// <returns></returns>
        [HttpPost, Route("relas/{roleId}"), MethodLog("重置角色权限关联信息")]
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_DIST)]
        public async Task<ApiRes> RelasAsync(string roleId, List<string> entIds)
        {
            var role = await _sysRoleService.GetByIdAsync(roleId);
            if (role == null || !role.SysType.Equals(CS.SYS_TYPE.MCH) || !role.BelongInfoId.Equals(GetCurrentMchNo()))
            {
                ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (entIds.Count > 0)
            {
                _sysRoleEntRelaService.ResetRela(roleId, entIds);

                //查询到该角色的人员， 将redis更新
                var sysUserIdList = _sysUserRoleRelaService.SelectUserIdsByRoleId(roleId).ToList();
                RefAuthentication(sysUserIdList);
            }
            return ApiRes.Ok();
        }
    }
}
