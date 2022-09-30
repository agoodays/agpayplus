using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AGooday.AgPay.Manager.Api.Controllers.Config
{
    [Route("/api/sysConfigs")]
    [ApiController]
    public class SysConfigController : ControllerBase
    {
        private readonly ISysConfigService _sysConfigService;

        /// <summary>
        /// 分组下的配置
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        [HttpGet, Route("{groupKey}")]
        public ApiRes GetConfigs(string groupKey)
        {
            var configList = _sysConfigService.GetAll()
                .Where(w => string.IsNullOrWhiteSpace(groupKey) || w.GroupKey.Equals(groupKey))
                .OrderBy(o => o.SortNum);
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 系统配置修改
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("{groupKey}")]
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

            return ApiRes.Ok();
        }

        private void UpdateSysConfigMQ(string groupKey) { 
        
        }
    }
}
