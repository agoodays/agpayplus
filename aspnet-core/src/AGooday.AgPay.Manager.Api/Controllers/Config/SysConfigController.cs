using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Config
{
    [Route("/api/sysConfigs")]
    [ApiController, Authorize]
    public class SysConfigController : ControllerBase
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<SysConfigController> _logger;
        private readonly ISysConfigService _sysConfigService;

        public SysConfigController(IMQSender mqSender,
            ILogger<SysConfigController> logger,
            ISysConfigService sysConfigService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 分组下的配置
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        [HttpGet, Route("{groupKey}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_SYS_CONFIG_INFO)]
        public ApiRes GetConfigs(string groupKey)
        {
            var configList = _sysConfigService.GetByKey(groupKey);
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 系统配置修改
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("{groupKey}"), MethodLog("系统配置修改")]
        [PermissionAuth(PermCode.MGR.ENT_SYS_CONFIG_EDIT)]
        public ApiRes Update(string groupKey, Dictionary<string, string> configs)
        {
            //foreach (var config in configs)
            //{
            //    _sysConfigService.SaveOrUpdate(new SysConfigDto() { ConfigKey = config.Key, ConfigVal = config.Value });
            //}
            int update = _sysConfigService.UpdateByConfigKey(configs);
            if (update <= 0)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "更新失败");
            }
            // 异步更新到MQ
            UpdateSysConfigMQ(groupKey);

            return ApiRes.Ok();
        }

        private async void UpdateSysConfigMQ(string groupKey)
        {
            await Task.Run(() =>
            {
                mqSender.Send(ResetAppConfigMQ.Build(groupKey));
            });
        }
    }
}
