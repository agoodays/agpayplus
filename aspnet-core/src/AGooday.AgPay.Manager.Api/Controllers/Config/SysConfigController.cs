using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Extensions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Manager.Api.Controllers.Config
{
    [Route("api/sysConfigs")]
    [ApiController, Authorize]
    public class SysConfigController : CommonController
    {
        private readonly IMQSender _mqSender;
        private readonly ISysConfigService _sysConfigService;

        public SysConfigController(ILogger<SysConfigController> logger,
            IMQSender mqSender,
            ISysConfigService sysConfigService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _mqSender = mqSender;
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
            var configList = _sysConfigService.GetByGroupKey(groupKey, CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);

            // 敏感数据脱敏
            if (groupKey.Equals("ossConfig"))
            {
                var sysConfig = configList.FirstOrDefault(w => w.ConfigKey.Equals("aliyunOssConfig"));
                if (sysConfig != null)
                {
                    JsonConfigValDesen(sysConfig, "accessKeySecret");
                }
            }
            if (groupKey.Equals("apiMapConfig"))
            {
                ConfigValDesen(configList);
            }
            if (groupKey.Equals("smsConfig"))
            {
                var sysConfig = configList.FirstOrDefault(w => w.ConfigKey.Equals("agpaydxSmsConfig"));
                if (sysConfig != null)
                {
                    JsonConfigValDesen(sysConfig, "accountPwd");
                }
                sysConfig = configList.FirstOrDefault(w => w.ConfigKey.Equals("aliyundySmsConfig"));
                if (sysConfig != null)
                {
                    JsonConfigValDesen(sysConfig, "accessKeySecret");
                }
            }
            if (groupKey.Equals("ocrConfig"))
            {
                var sysConfig = configList.FirstOrDefault(w => w.ConfigKey.Equals("tencentOcrConfig"));
                if (sysConfig != null)
                {
                    JsonConfigValDesen(sysConfig, "secretKey");
                }
                sysConfig = configList.FirstOrDefault(w => w.ConfigKey.Equals("aliOcrConfig"));
                if (sysConfig != null)
                {
                    JsonConfigValDesen(sysConfig, "accessKeySecret");
                }
                sysConfig = configList.FirstOrDefault(w => w.ConfigKey.Equals("baiduOcrConfig"));
                if (sysConfig != null)
                {
                    JsonConfigValDesen(sysConfig, "aecretKey");
                }
            }

            return ApiRes.Ok(configList);
        }

        private static void ConfigValDesen(IEnumerable<SysConfigDto> configList)
        {
            var sysConfig = configList.FirstOrDefault(w => w.ConfigKey.Equals("apiMapWebSecret"));
            if (sysConfig != null)
            {
                sysConfig.AddExt("configValDesen", sysConfig.ConfigVal?.Mask());
                sysConfig.ConfigVal = null;
            }
        }

        private static void JsonConfigValDesen(SysConfigDto sysConfig, string key)
        {
            var configVal = JObject.Parse(sysConfig.ConfigVal);
            configVal.TryGetString(key, out string accountPwd);
            configVal[key] = accountPwd.Mask();
            sysConfig.AddExt("configValDesen", JsonConvert.SerializeObject(configVal));
            configVal.Remove(key);
            sysConfig.ConfigVal = JsonConvert.SerializeObject(configVal);
        }

        /// <summary>
        /// 系统配置修改
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("{groupKey}"), MethodLog("系统配置修改")]
        [PermissionAuth(PermCode.MGR.ENT_SYS_CONFIG_EDIT)]
        public async Task<ApiRes> UpdateAsync(string groupKey, Dictionary<string, string> configs)
        {
            //foreach (var config in configs)
            //{
            //    _sysConfigService.SaveOrUpdate(new SysConfigDto() { ConfigKey = config.Key, ConfigVal = config.Value });
            //}
            int update = await _sysConfigService.UpdateByConfigKeyAsync(configs, groupKey, CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);
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
            return _mqSender.SendAsync(ResetAppConfigMQ.Build(groupKey));
        }
    }
}
