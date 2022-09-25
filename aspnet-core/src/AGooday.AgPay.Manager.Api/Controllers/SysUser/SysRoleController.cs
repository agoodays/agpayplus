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
using AGooday.AgPay.Manager.Api.Models;
using Newtonsoft.Json;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("/api/sysRoles")]
    [ApiController]
    public class SysRoleController : CommonController
    {
        private readonly ILogger<SysRoleController> _logger;
        private readonly IDatabase _redis;
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
        [HttpGet]
        [Route("")]
        public ApiRes List([FromQuery] SysRoleQueryDto dto)
        {
            var data = _sysRoleService.GetPaginatedData(dto);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ApiRes Add(SysRoleCreateDto dto)
        {
            dto.RoleId = $"ROLE_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
            dto.SysType = CS.SYS_TYPE.MGR;
            dto.BelongInfoId = "0";
            _sysRoleService.Add(dto);

            //如果包含： 可分配权限的权限 && EntIds 不为空
            if (GetCurrentUser().Authorities.Contains(PermCode.MGR.ENT_UR_ROLE_DIST) && dto.EntIds?.Count > 0)
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
        [HttpDelete]
        [Route("{recordId}")]
        public ApiRes Delete(string recordId)
        {
            _sysRoleService.RemoveRole(recordId);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{recordId}")]
        public ApiRes Update(SysRoleModifyDto dto)
        {
            _sysRoleService.Update(dto);
            //如果包含： 可分配权限的权限 && EntIds 不为空
            if (GetCurrentUser().Authorities.Contains(PermCode.MGR.ENT_UR_ROLE_DIST) && dto.EntIds?.Count > 0)
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
        [HttpGet]
        [Route("{recordId}")]
        public ApiRes Detail(string recordId)
        {
            var sysRole = _sysRoleService.GetById(recordId);
            if (sysRole is null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysRole);
        }
    }
}
