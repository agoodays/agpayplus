using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers.Agent
{
    [Route("/api/agentConfig")]
    [ApiController, Authorize]
    public class AgentConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<AgentConfigController> _logger;
        private readonly IAgentInfoService _agentInfoService;
        private readonly ISysConfigService _sysConfigService;

        public AgentConfigController(IMQSender mqSender,
            ILogger<AgentConfigController> logger,
            IAgentInfoService agentInfoService,
            ISysConfigService sysConfigService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _agentInfoService = agentInfoService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 分组下的配置
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        [HttpGet, Route("{groupKey}"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_AGENT_CONFIG)]
        public ApiRes GetConfigs(string groupKey)
        {
            var configList = _sysConfigService.GetByGroupKey(groupKey, CS.SYS_TYPE.AGENT, GetCurrentAgentNo());
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 更新商户配置信息
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("{groupKey}"), MethodLog("更新商户配置信息")]
        [PermissionAuth(PermCode.AGENT.ENT_AGENT_CONFIG_EDIT)]
        public ApiRes Update(string groupKey, Dictionary<string, string> configs)
        {
            int update = _sysConfigService.UpdateByConfigKey(configs, groupKey, CS.SYS_TYPE.AGENT, GetCurrentAgentNo());
            if (update <= 0)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "更新失败");
            }

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更改支付密码	
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("agentSipw"), MethodLog("更改支付密码")]
        [PermissionAuth(PermCode.AGENT.ENT_AGENT_CONFIG_EDIT)]
        public ApiRes SetMchSipw(ModifyAgentSipw model)
        {
            var agentInfo = _agentInfoService.GetById(GetCurrentAgentNo());
            string currentSipw = Base64Util.DecodeBase64(model.OriginalPwd);
            bool verified = BCrypt.Net.BCrypt.Verify(currentSipw, agentInfo.Sipw);
            //验证当前密码是否正确
            if (!verified)
            {
                throw new BizException("原支付密码验证失败！");
            }
            string opSipw = Base64Util.DecodeBase64(model.ConfirmPwd);
            // 验证原密码与新密码是否相同
            if (opSipw.Equals(currentSipw))
            {
                throw new BizException("新密码与原密码不能相同！");
            }
            AgentInfoUpdateDto dto = new AgentInfoUpdateDto();
            dto.AgentNo = agentInfo.AgentNo;
            dto.Sipw = opSipw;
            _agentInfoService.UpdateById(dto);
            return ApiRes.Ok();
        }
    }
}
