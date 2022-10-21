using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Notifications;
using MediatR;
using AGooday.AgPay.Application.Permissions;
using Newtonsoft.Json;
using StackExchange.Redis;
using AGooday.AgPay.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using AGooday.AgPay.Merchant.Api.Authorization;

namespace AGooday.AgPay.Merchant.Api.Controllers.SysUser
{

    /// <summary>
    /// 角色 权限管理
    /// </summary>
    [Route("/api/sysRoleEntRelas")]
    [ApiController, Authorize]
    public class SysRoleEntRelaController : CommonController
    {
        private readonly ILogger<SysRoleEntRelaController> _logger;
        private readonly ISysRoleService _sysRoleService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysRoleEntRelaController(ILogger<SysRoleEntRelaController> logger, RedisUtil client,
            ISysRoleService sysRoleService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
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
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_DIST)]
        public ApiRes List([FromQuery] SysRoleEntRelaQueryDto dto)
        {
            var data = _sysRoleEntRelaService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        [HttpPost, Route("relas/{roleId}")]
        [PermissionAuth(PermCode.MCH.ENT_UR_ROLE_DIST)]
        public ApiRes Relas(string roleId, List<string> entIds)
        {
            var role = _sysRoleService.GetById(roleId);
            if (role == null || !role.SysType.Equals(CS.SYS_TYPE.MCH) || !role.BelongInfoId.Equals(GetCurrentUser().User.BelongInfoId))
            {
                ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (entIds.Count() > 0)
            {
                _sysRoleEntRelaService.ResetRela(roleId, entIds);

                //查询到该角色的人员， 将redis更新
                var sysUserIdList = _sysUserRoleRelaService.SelectRoleIdsByRoleId(roleId).ToList();
                RefAuthentication(sysUserIdList);
            }
            return ApiRes.Ok();
        }
    }
}
