using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Merchant.Api.Authorization;
using AGooday.AgPay.Merchant.Api.Attributes;

namespace AGooday.AgPay.Merchant.Api.Controllers.SysUser
{
    /// <summary>
    /// 用户角色管理类
    /// </summary>
    [Route("/api/sysUserRoleRelas")]
    [ApiController, Authorize]
    public class SysUserRoleRelaController : CommonController
    {
        private readonly ILogger<SysUserRoleRelaController> _logger;
        private readonly ISysUserService _sysUserService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysUserRoleRelaController(ILogger<SysUserRoleRelaController> logger, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
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
        public ApiRes List([FromQuery] SysUserRoleRelaQueryDto dto)
        {
            var data = _sysUserRoleRelaService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
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
            if (entIds.Count() > 0)
            {
                _sysUserRoleRelaService.SaveUserRole(sysUserId, entIds);
                RefAuthentication(new List<long> { sysUserId });
            }
            return ApiRes.Ok();
        }
    }
}
