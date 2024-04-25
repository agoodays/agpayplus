using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers.Agent
{
    [Route("/api/agentConfig")]
    [ApiController, Authorize]
    public class AgentConfigController : CommonController
    {
        private readonly IAgentInfoService _agentInfoService;

        public AgentConfigController(ILogger<AgentConfigController> logger,
            IAgentInfoService agentInfoService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _agentInfoService = agentInfoService;
        }

        /// <summary>
        /// 更改支付密码	
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("agentSipw"), MethodLog("更改支付密码")]
        [PermissionAuth(PermCode.AGENT.ENT_AGENT_CONFIG_EDIT)]
        public ApiRes SetAgentSipw(ModifyAgentSipw model)
        {
            var agentInfo = _agentInfoService.GetById(GetCurrentAgentNo());
            string currentSipw = Base64Util.DecodeBase64(model.OriginalPwd);
            if (!string.IsNullOrWhiteSpace(agentInfo.Sipw))
            {
                bool verified = BCryptUtil.VerifyHash(currentSipw, agentInfo.Sipw);
                //验证当前密码是否正确
                if (!verified)
                {
                    throw new BizException("原支付密码验证失败！");
                }
            }
            string opSipw = Base64Util.DecodeBase64(model.ConfirmPwd);
            // 验证原密码与新密码是否相同
            if (opSipw.Equals(currentSipw))
            {
                throw new BizException("新密码与原密码不能相同！");
            }
            agentInfo.Sipw = opSipw;
            _agentInfoService.UpdateById(agentInfo);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 获取是否设置支付密码
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("hasSipwValidate"), NoLog]
        public ApiRes HasSipwValidate()
        {
            var agentInfo = _agentInfoService.GetById(GetCurrentAgentNo());
            return ApiRes.Ok(!string.IsNullOrWhiteSpace(agentInfo.Sipw));
        }
    }
}
