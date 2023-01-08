using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers.SysUser
{
    /// <summary>
    /// 用户团队管理类
    /// </summary>
    [Route("/api/userTeams")]
    [ApiController, Authorize]
    public class SysUserTeamController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<SysUserTeamController> _logger;
        private readonly ISysUserTeamService _mchStoreService;

        public SysUserTeamController(IMQSender mqSender, ILogger<SysUserTeamController> logger,
            ISysUserTeamService mchStoreService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _mchStoreService = mchStoreService;
        }

        /// <summary>
        /// 团队列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_LIST)]
        public ApiRes List([FromQuery] SysUserTeamQueryDto dto)
        {
            dto.SysType = CS.SYS_TYPE.AGENT;
            dto.BelongInfoId = GetCurrentAgentNo();
            var data = _mchStoreService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 新建团队
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建团队")]
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_ADD)]
        public ApiRes Add(SysUserTeamDto dto)
        {
            dto.SysType = CS.SYS_TYPE.AGENT;
            dto.BelongInfoId = GetCurrentAgentNo();
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
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_VIEW, PermCode.AGENT.ENT_UR_TEAM_EDIT)]
        public ApiRes Delete(long recordId)
        {
            var mchStore = _mchStoreService.GetById(recordId);
            _mchStoreService.Remove(recordId);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新团队信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新团队信息")]
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_EDIT)]
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
        [PermissionAuth(PermCode.AGENT.ENT_UR_TEAM_DEL)]
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
