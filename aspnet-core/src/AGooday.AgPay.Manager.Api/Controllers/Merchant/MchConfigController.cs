using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    [Route("/api/mchConfig")]
    [ApiController, Authorize]
    public class MchConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ISysConfigService _sysConfigService;

        public MchConfigController(ILogger<MchConfigController> logger,
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
        [PermissionAuth(PermCode.MGR.ENT_MCH_CONFIG_PAGE)]
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
        [PermissionAuth(PermCode.MGR.ENT_MCH_CONFIG_PAGE)]
        public ApiRes Update(string groupKey, MchConfigRequest model)
        {
            int update = _sysConfigService.UpdateByConfigKey(model.Configs, groupKey, CS.SYS_TYPE.MCH, model.MchNo);
            if (update <= 0)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "更新失败");
            }

            return ApiRes.Ok();
        }
    }
}
