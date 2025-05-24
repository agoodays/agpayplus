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

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/sysRoles")]
    [ApiController, Authorize]
    public class SysRoleController : CommonController
    {
        private readonly ISysRoleService _sysRoleService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysRoleController(ILogger<SysRoleController> logger,
            ICacheService cacheService,
            IAuthService authService,
            ISysRoleService sysRoleService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, cacheService, authService)
        {
            _sysRoleService = sysRoleService;
            _sysRoleEntRelaService = sysRoleEntRelaService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
        }

        /// <summary>
        /// 角色信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_LIST, PermCode.MGR.ENT_UR_USER_UPD_ROLE)]
        public async Task<ApiPageRes<SysRoleDto>> ListAsync([FromQuery] SysRoleQueryDto dto)
        {
            dto.BelongInfoId = string.IsNullOrWhiteSpace(dto.BelongInfoId) ? (dto.SysType ?? string.Empty).Equals(CS.SYS_TYPE.MGR) ? CS.BASE_BELONG_INFO_ID.MGR : dto.BelongInfoId : dto.BelongInfoId;
            var data = await _sysRoleService.GetPaginatedDataAsync(dto);
            return ApiPageRes<SysRoleDto>.Pages(data);
        }

        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("添加角色信息")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_ADD)]
        public async Task<ApiRes> AddAsync(SysRoleCreateDto dto)
        {
            dto.RoleId = $"ROLE_{StringUtil.GetUUID(6)}";
            dto.SysType = CS.SYS_TYPE.MGR;
            dto.BelongInfoId = CS.BASE_BELONG_INFO_ID.MGR;
            await _sysRoleService.AddAsync(dto);

            //如果包含： 可分配权限的权限 && EntIds 不为空
            if ((await GetCurrentUserAsync()).Authorities.Contains(PermCode.MGR.ENT_UR_ROLE_DIST))
            {
                await _sysRoleEntRelaService.ResetRelaAsync(dto.RoleId, dto.EntIds);
            }

            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除角色")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_DEL)]
        public async Task<ApiRes> DeleteAsync(string recordId)
        {
            await _sysRoleService.RemoveRoleAsync(recordId);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新角色信息")]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_EDIT)]
        public async Task<ApiRes> UpdateAsync(string recordId, SysRoleModifyDto dto)
        {
            await _sysRoleService.UpdateAsync(dto);
            //如果包含： 可分配权限的权限 && EntIds 不为空
            if ((await GetCurrentUserAsync()).Authorities.Contains(PermCode.MGR.ENT_UR_ROLE_DIST))
            {
                await _sysRoleEntRelaService.ResetRelaAsync(dto.RoleId, dto.EntIds);

                //查询到该角色的人员， 将redis更新
                var sysUserIdList = _sysUserRoleRelaService.SelectUserIdsByRoleId(dto.RoleId).ToList();
                await RefAuthenticationAsync(sysUserIdList);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看角色信息
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_ROLE_EDIT)]
        public async Task<ApiRes> DetailAsync(string recordId)
        {
            var sysRole = await _sysRoleService.GetByIdAsNoTrackingAsync(recordId);
            if (sysRole is null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysRole);
        }
    }
}
