﻿using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers.Config
{
    [Route("api/sysConfigs")]
    [ApiController, Authorize]
    public class SysConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ISysConfigService _sysConfigService;

        public SysConfigController(ILogger<SysConfigController> logger,
            IMQSender mqSender,
            ISysConfigService sysConfigService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            this.mqSender = mqSender;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 分组下的配置
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        [HttpGet, Route("{groupKey}"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_SYS_CONFIG_INFO)]
        public ApiRes GetConfigs(string groupKey)
        {
            var configList = _sysConfigService.GetByGroupKey(groupKey, CS.SYS_TYPE.AGENT, GetCurrentAgentNo());
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 系统配置修改
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("{groupKey}"), MethodLog("系统配置修改")]
        [PermissionAuth(PermCode.AGENT.ENT_SYS_CONFIG_EDIT)]
        public async Task<ApiRes> Update(string groupKey, Dictionary<string, string> configs)
        {
            //foreach (var config in configs)
            //{
            //    _sysConfigService.SaveOrUpdate(new SysConfigDto() { ConfigKey = config.Key, ConfigVal = config.Value });
            //}
            int update = _sysConfigService.UpdateByConfigKey(configs, groupKey, CS.SYS_TYPE.AGENT, GetCurrentAgentNo());
            if (update <= 0)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "更新失败");
            }
            // 异步更新到MQ
            await UpdateSysConfigMQAsync(groupKey);

            return ApiRes.Ok();
        }

        private Task UpdateSysConfigMQAsync(string groupKey)
        {
            return mqSender.SendAsync(ResetAppConfigMQ.Build(groupKey));
        }
    }
}
