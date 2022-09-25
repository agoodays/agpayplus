using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Config
{
    [Route("/api/sysConfigs")]
    [ApiController]
    public class SysConfigController : ControllerBase
    {
        private readonly ISysConfigService _sysConfigService;

        [HttpGet, Route("{groupKey}")]
        public ApiRes GetConfigs(string groupKey)
        {
            var configList = _sysConfigService.GetAll()
                .Where(w => string.IsNullOrWhiteSpace(groupKey) || w.GroupKey.Equals(groupKey))
                .OrderBy(o => o.SortNum);
            return ApiRes.Ok(configList);
        }

        [HttpPut, Route("update/{groupKey}")]
        public ApiRes Update(string groupKey, Dictionary<string, string> configs)
        {
            foreach (var config in configs)
            {
                _sysConfigService.SaveOrUpdate(new SysConfigDto() { ConfigKey = config.Key, ConfigVal = config.Value });
            }
            return ApiRes.Ok();
        }
    }
}
