using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Merchant
{
    [Route("/api/mchConfig")]
    [ApiController, Authorize]
    public class MchConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<MchConfigController> _logger;
        private readonly ISysConfigService _sysConfigService;

        public MchConfigController(IMQSender mqSender,
            ILogger<MchConfigController> logger,
            ISysConfigService sysConfigService, RedisUtil client,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
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
        [PermissionAuth(PermCode.MCH.ENT_MCH_CONFIG)]
        public ApiRes GetConfigs(string groupKey)
        {
            var configList = _sysConfigService.GetByGroupKey(groupKey, CS.SYS_TYPE.MCH, GetCurrentMchNo());
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 更新商户配置信息
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("{groupKey}"), MethodLog("更新商户配置信息")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_CONFIG_EDIT)]
        public ApiRes Update(string groupKey, Dictionary<string, string> configs)
        {
            int update = _sysConfigService.UpdateByConfigKey(configs, groupKey, CS.SYS_TYPE.MCH, GetCurrentMchNo());
            if (update <= 0)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "更新失败");
            }

            return ApiRes.Ok();
        }
    }
}
