using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.DataTransfer;
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
        public ApiRes SetMchSipw(ModifyAgentSipw model)
        {
            var agentInfo = _agentInfoService.GetById(GetCurrentAgentNo());
            string currentSipw = Base64Util.DecodeBase64(model.OriginalPwd);
            bool verified = BCryptUtil.VerifyHash(currentSipw, agentInfo.Sipw);
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
