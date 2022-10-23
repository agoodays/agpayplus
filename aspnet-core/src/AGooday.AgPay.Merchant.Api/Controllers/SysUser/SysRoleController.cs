using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Merchant.Api.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using AGooday.AgPay.Merchant.Api.Authorization;

namespace AGooday.AgPay.Merchant.Api.Controllers.SysUser
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("/api/sysRoles")]
    [ApiController, Authorize]
    public class SysRoleController : CommonController
    {
        private readonly ILogger<SysRoleController> _logger;
        private readonly ISysRoleService _sysRoleService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysRoleController(ILogger<SysRoleController> logger, RedisUtil client,
            ISysRoleService sysRoleService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _sysRoleService = sysRoleService;
            _sysRoleEntRelaService = sysRoleEntRelaService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
        }

        /// <summary>
        /// 角色信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_LIST, PermCode.MCH.ENT_UR_USER_UPD_ROLE)]
        public ApiRes List([FromQuery] SysRoleQueryDto dto)
        {
            dto.SysType = CS.SYS_TYPE.MCH;
            dto.BelongInfoId = GetCurrentUser().User.BelongInfoId;
            var data = _sysRoleService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_ADD)]
        public ApiRes Add(SysRoleCreateDto dto)
        {
            dto.RoleId = $"ROLE_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
            dto.SysType = CS.SYS_TYPE.MCH;
            dto.BelongInfoId = GetCurrentUser().User.BelongInfoId;
            _sysRoleService.Add(dto);

            //如果包含： 可分配权限的权限 && EntIds 不为空
            if (GetCurrentUser().Authorities.Contains(PermCode.MCH.ENT_UR_ROLE_DIST) && dto.EntIds?.Count > 0)
            {
                _sysRoleEntRelaService.ResetRela(dto.RoleId, dto.EntIds);
            }

            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}")]
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_DEL)]
        public ApiRes Delete(string recordId)
        {
            var sysRole = _sysRoleService.GetById(recordId, GetCurrentUser().User.BelongInfoId);
            if (sysRole is null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            _sysRoleService.RemoveRole(recordId);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}")]
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_EDIT)]
        public ApiRes Update(string recordId, SysRoleModifyDto dto)
        {
            _sysRoleService.Update(dto);
            //如果包含： 可分配权限的权限 && EntIds 不为空
            if (GetCurrentUser().Authorities.Contains(PermCode.MCH.ENT_UR_ROLE_DIST) && dto.EntIds?.Count > 0)
            {
                _sysRoleEntRelaService.ResetRela(dto.RoleId, dto.EntIds);

                //查询到该角色的人员， 将redis更新
                var sysUserIdList = _sysUserRoleRelaService.SelectRoleIdsByRoleId(dto.RoleId).ToList();
                RefAuthentication(sysUserIdList);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看角色信息
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}")]
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_EDIT)]
        public ApiRes Detail(string recordId)
        {
            var sysRole = _sysRoleService.GetById(recordId, GetCurrentUser().User.BelongInfoId);
            if (sysRole is null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysRole);
        }
    }
}
