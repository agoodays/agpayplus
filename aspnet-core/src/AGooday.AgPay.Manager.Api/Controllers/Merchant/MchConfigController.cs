using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    [Route("/api/mchConfig")]
    [ApiController, Authorize]
    public class MchConfigController : ControllerBase
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<MchConfigController> _logger;
        private readonly ISysConfigService _sysConfigService;

        public MchConfigController(IMQSender mqSender,
            ILogger<MchConfigController> logger,
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
        public ApiRes GetConfigs(string groupKey, string mchNo)
        {
            var configList = _sysConfigService.GetByGroupKey(groupKey, CS.SYS_TYPE.MCH, mchNo);
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 更新商户配置信息
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("{groupKey}"), MethodLog("更新商户配置信息")]
        [PermissionAuth(PermCode.MGR.ENT_SYS_CONFIG_EDIT)]
        public ApiRes Update(string groupKey, [FromBody] MchConfigRequest request)
        {
            int update = _sysConfigService.UpdateByConfigKey(request.Configs, groupKey, CS.SYS_TYPE.MCH, request.MchNo);
            if (update <= 0)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "更新失败");
            }

            return ApiRes.Ok();
        }
    }
}
