using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Agent.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户管理类
    /// </summary>
    [Route("api/mchInfo")]
    [ApiController, Authorize]
    public class MchInfoController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly IMchInfoService _mchInfoService;
        private readonly IAgentInfoService _agentInfoService;
        private readonly ISysUserService _sysUserService;

        private readonly DomainNotificationHandler _notifications;

        public MchInfoController(ILogger<MchInfoController> logger,
            IMQSender mqSender,
            INotificationHandler<DomainNotification> notifications,
            IMchInfoService mchInfoService,
            IAgentInfoService agentInfoService,
            ISysUserService sysUserService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            this.mqSender = mqSender;
            _mchInfoService = mchInfoService;
            _agentInfoService = agentInfoService;
            _sysUserService = sysUserService;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 商户信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_LIST)]
        public ApiPageRes<MchInfoDto> List([FromQuery] MchInfoQueryDto dto)
        {
            dto.AgentNo = GetCurrentAgentNo();
            var data = _mchInfoService.GetPaginatedData(dto);
            return ApiPageRes<MchInfoDto>.Pages(data);
        }

        /// <summary>
        /// 新增商户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增商户信息")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_INFO_ADD)]
        public async Task<ApiRes> AddAsync(MchInfoCreateDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            var agentNo = sysUser.BelongInfoId;
            var agentInfos = _agentInfoService.GetParents(agentNo);
            var topAgentInfo = agentInfos.OrderBy(x => x.Level).FirstOrDefault();
            var agentInfo = agentInfos.FirstOrDefault(f => f.AgentNo.Equals(agentNo));
            dto.RefundMode = JArray.Parse("[\"plat\", \"api\"]");
            dto.Type = CS.MCH_TYPE_ISVSUB;
            dto.TopAgentNo = topAgentInfo.AgentNo;
            dto.AgentNo = agentInfo.AgentNo;
            dto.IsvNo = agentInfo.IsvNo;
            await _mchInfoService.CreateAsync(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 删除商户信息
        /// </summary>
        /// <param name="mchNo"></param>
        /// <returns></returns>
        [HttpDelete, Route("{mchNo}"), MethodLog("删除商户信息")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_INFO_DEL)]
        public async Task<ApiRes> DeleteAsync(string mchNo)
        {
            await _mchInfoService.RemoveAsync(mchNo);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{mchNo}"), MethodLog("更新商户信息")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_INFO_EDIT)]
        public async Task<ApiRes> UpdateAsync(string mchNo, MchInfoModifyDto dto)
        {
            await _mchInfoService.ModifyAsync(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 查询商户信息
        /// </summary>
        /// <param name="mchNo"></param>
        /// <returns></returns>
        [HttpGet, Route("{mchNo}"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_INFO_VIEW, PermCode.AGENT.ENT_MCH_INFO_EDIT)]
        public async Task<ApiRes> DetailAsync(string mchNo)
        {
            var mchInfo = await _mchInfoService.GetByIdAsync(mchNo);
            if (mchInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var sysUser = await _sysUserService.GetByIdAsync(mchInfo.InitUserId.Value);
            if (sysUser != null)
            {
                mchInfo.AddExt("loginUsername", sysUser.LoginUsername);
            }
            return ApiRes.Ok(mchInfo);
        }
    }
}
