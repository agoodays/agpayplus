using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers.SysUser
{
    /// <summary>
    /// 用户团队管理类
    /// </summary>
    [Route("api/userTeams")]
    [ApiController, Authorize]
    public class SysUserTeamController : CommonController
    {
        private readonly ISysUserTeamService _sysUserTeamService;

        public SysUserTeamController(ILogger<SysUserTeamController> logger,
            ISysUserTeamService mchStoreService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _sysUserTeamService = mchStoreService;
        }

        /// <summary>
        /// 团队列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_LIST)]
        public async Task<ApiPageRes<SysUserTeamDto>> ListAsync([FromQuery] SysUserTeamQueryDto dto)
        {
            dto.SysType = CS.SYS_TYPE.AGENT;
            dto.BelongInfoId = GetCurrentAgentNo();
            var data = await _sysUserTeamService.GetPaginatedDataAsync(dto);
            return ApiPageRes<SysUserTeamDto>.Pages(data);
        }

        /// <summary>
        /// 新建团队
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建团队")]
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_ADD)]
        public async Task<ApiRes> AddAsync(SysUserTeamDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            dto.SysType = CS.SYS_TYPE.AGENT;
            dto.BelongInfoId = sysUser.BelongInfoId;
            var result = await _sysUserTeamService.AddAsync(dto);
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
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_DEL)]
        public async Task<ApiRes> DeleteAsync(long recordId)
        {
            var mchStore = await _sysUserTeamService.GetByIdAsync(recordId);
            if (mchStore == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            await _sysUserTeamService.RemoveAsync(recordId);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新团队信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新团队信息")]
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_EDIT)]
        public async Task<ApiRes> UpdateAsync(long recordId, SysUserTeamDto dto)
        {
            var result = await _sysUserTeamService.UpdateAsync(dto);
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
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_VIEW, PermCode.AGENT.ENT_UR_TEAM_EDIT)]
        public async Task<ApiRes> DetailAsync(long recordId)
        {
            var mchStore = await _sysUserTeamService.GetByIdAsync(recordId);
            if (mchStore == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchStore);
        }
    }
}
