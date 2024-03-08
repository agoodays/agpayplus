using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.SysUser
{
    /// <summary>
    /// 用户角色管理类
    /// </summary>
    [Route("/api/sysUserRoleRelas")]
    [ApiController, Authorize]
    public class SysUserRoleRelaController : CommonController
    {
        private readonly ISysUserService _sysUserService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysUserRoleRelaController(ILogger<SysUserRoleRelaController> logger,
            ISysUserService sysUserService,
            ISysUserRoleRelaService sysUserRoleRelaService, 
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _sysUserService = sysUserService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
        }

        /// <summary>
        /// 用户角色列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_UR_USER_UPD_ROLE)]
        public ApiPageRes<SysUserRoleRelaDto> List([FromQuery] SysUserRoleRelaQueryDto dto)
        {
            var data = _sysUserRoleRelaService.GetPaginatedData(dto);
            return ApiPageRes<SysUserRoleRelaDto>.Pages(data);
        }

        /// <summary>
        /// 重置用户角色关联信息
        /// </summary>
        /// <param name="sysUserId"></param>
        /// <returns></returns>
        [HttpPost, Route("relas/{sysUserId}"), MethodLog("重置用户角色关联信息")]
        [PermissionAuth(PermCode.MCH.ENT_UR_USER_UPD_ROLE)]
        public ApiRes Relas(long sysUserId, List<string> entIds)
        {
            var dbRecord = _sysUserService.GetById(sysUserId, GetCurrentMchNo());
            if (dbRecord == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (entIds.Count > 0)
            {
                _sysUserRoleRelaService.SaveUserRole(sysUserId, entIds);
                RefAuthentication(new List<long> { sysUserId });
            }
            return ApiRes.Ok();
        }
    }
}
