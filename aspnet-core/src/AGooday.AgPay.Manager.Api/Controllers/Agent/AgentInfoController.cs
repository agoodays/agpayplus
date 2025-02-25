using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Agent
{
    /// <summary>
    /// 代理商管理类
    /// </summary>
    [Route("api/agentInfo")]
    [ApiController, Authorize]
    public class AgentInfoController : CommonController
    {
        private readonly IMQSender _mqSender;
        private readonly IAgentInfoService _agentInfoService;
        private readonly ISysUserService _sysUserService;

        private readonly DomainNotificationHandler _notifications;

        public AgentInfoController(IMQSender mqSender, ILogger<AgentInfoController> logger,
            ICacheService cacheService,
            IAuthService authService,
            INotificationHandler<DomainNotification> notifications,
            IAgentInfoService agentInfoService,
            ISysUserService sysUserService)
            : base(logger, cacheService, authService)
        {
            _mqSender = mqSender;
            _agentInfoService = agentInfoService;
            _sysUserService = sysUserService;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 代理商信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_AGENT_LIST, PermCode.MGR.ENT_MCH_INFO_ADD, PermCode.MGR.ENT_MCH_INFO_EDIT, PermCode.MGR.ENT_MCH_INFO_VIEW)]
        public async Task<ApiPageRes<AgentInfoDto>> ListAsync([FromQuery] AgentInfoQueryDto dto)
        {
            var data = await _agentInfoService.GetPaginatedDataAsync(dto);
            return ApiPageRes<AgentInfoDto>.Pages(data);
        }

        /// <summary>
        /// 新增代理商信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增代理商信息")]
        [PermissionAuth(PermCode.MGR.ENT_AGENT_INFO_ADD)]
        public async Task<ApiRes> AddAsync(AgentInfoCreateDto dto)
        {
            var sysUser = (await GetCurrentUserAsync()).SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            await _agentInfoService.CreateAsync(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 删除代理商信息
        /// </summary>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        [HttpDelete, Route("{agentNo}"), MethodLog("删除代理商信息")]
        [PermissionAuth(PermCode.MGR.ENT_AGENT_INFO_DEL)]
        public async Task<ApiRes> DeleteAsync(string agentNo)
        {
            await _agentInfoService.RemoveAsync(agentNo);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新代理商信息
        /// </summary>
        /// <param name="agentNo"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{agentNo}"), MethodLog("更新代理商信息")]
        [PermissionAuth(PermCode.MGR.ENT_AGENT_INFO_EDIT)]
        public async Task<ApiRes> UpdateAsync(string agentNo, AgentInfoModifyDto dto)
        {
            await _agentInfoService.ModifyAsync(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 查询代理商信息
        /// </summary>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        [HttpGet, Route("{agentNo}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_AGENT_INFO_VIEW, PermCode.MGR.ENT_AGENT_INFO_EDIT)]
        public async Task<ApiRes> DetailAsync(string agentNo)
        {
            var agentInfo = await _agentInfoService.GetByIdAsync(agentNo);
            if (agentInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var sysUser = await _sysUserService.GetByIdAsync(agentInfo.InitUserId.Value);
            if (sysUser != null)
            {
                agentInfo.AddExt("loginUsername", sysUser.LoginUsername);
            }
            return ApiRes.Ok(agentInfo);
        }
    }
}
