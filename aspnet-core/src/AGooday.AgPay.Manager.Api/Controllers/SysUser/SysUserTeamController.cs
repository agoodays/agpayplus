using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    /// <summary>
    /// 用户团队管理类
    /// </summary>
    [Route("api/userTeams")]
    [ApiController, Authorize]
    public class SysUserTeamController : CommonController
    {
        private readonly ISysUserTeamService _mchStoreService;

        public SysUserTeamController(ILogger<SysUserTeamController> logger,
            ISysUserTeamService mchStoreService, 
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _mchStoreService = mchStoreService;
        }

        /// <summary>
        /// 团队列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_TEAM_LIST)]
        public ApiPageRes<SysUserTeamDto> List([FromQuery] SysUserTeamQueryDto dto)
        {
            dto.BelongInfoId = string.IsNullOrWhiteSpace(dto.BelongInfoId) ? (dto.SysType ?? string.Empty).Equals(CS.SYS_TYPE.MGR) ? CS.BASE_BELONG_INFO_ID.MGR : dto.BelongInfoId : dto.BelongInfoId;
            var data = _mchStoreService.GetPaginatedData(dto);
            return ApiPageRes<SysUserTeamDto>.Pages(data);
        }

        /// <summary>
        /// 新建团队
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建团队")]
        [PermissionAuth(PermCode.MGR.ENT_UR_TEAM_ADD)]
        public ApiRes Add(SysUserTeamDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            dto.SysType = CS.SYS_TYPE.MGR;
            dto.BelongInfoId = CS.BASE_BELONG_INFO_ID.MGR;
            var result = _mchStoreService.Add(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除团队
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除团队")]
        [PermissionAuth(PermCode.MGR.ENT_UR_TEAM_DEL)]
        public ApiRes Delete(long recordId)
        {
            var mchStore = _mchStoreService.GetById(recordId);
            if (mchStore == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            _mchStoreService.Remove(recordId);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新团队信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新团队信息")]
        [PermissionAuth(PermCode.MGR.ENT_UR_TEAM_EDIT)]
        public ApiRes Update(long recordId, SysUserTeamDto dto)
        {
            var result = _mchStoreService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 团队详情
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_TEAM_VIEW, PermCode.MGR.ENT_UR_TEAM_EDIT)]
        public ApiRes Detail(long recordId)
        {
            var mchStore = _mchStoreService.GetById(recordId);
            if (mchStore == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchStore);
        }
    }
}
