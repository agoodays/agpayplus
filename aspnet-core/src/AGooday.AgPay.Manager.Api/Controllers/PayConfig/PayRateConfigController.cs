using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.PayConfig
{
    /// <summary>
    /// 支付费率接口管理类
    /// </summary>
    [Route("api/rateConfig")]
    [ApiController, Authorize]
    public class PayRateConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly IPayRateConfigService _payRateConfigService;

        public PayRateConfigController(ILogger<PayRateConfigController> logger,
            IMQSender mqSender,
            IPayRateConfigService payRateConfigService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            this.mqSender = mqSender;
            _payRateConfigService = payRateConfigService;
        }

        /// <summary>
        /// 查询支付接口配置列表
        /// </summary>
        /// <param name="infoId"></param>
        /// <param name="configMode"></param>
        /// <param name="ifName"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("savedMapData"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_LIST, PermCode.MGR.ENT_AGENT_PAY_CONFIG_LIST, PermCode.MGR.ENT_MCH_PAY_CONFIG_LIST)]
        public ApiRes SavedMapData(string configMode, string infoId, string ifCode)
        {
            return ApiRes.Ok(_payRateConfigService.GetByInfoIdAndIfCodeJson(configMode, infoId, ifCode));
        }

        /// <summary>
        /// 获取商户参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("payways"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_LIST, PermCode.MGR.ENT_AGENT_PAY_CONFIG_LIST, PermCode.MGR.ENT_MCH_PAY_CONFIG_LIST)]
        public async Task<ApiPageRes<PayWayDto>> GetPayWaysByInfoIdAsync([FromQuery] PayWayUsableQueryDto dto)
        {
            var data = await _payRateConfigService.GetPayWaysByInfoIdAsync(dto);
            return ApiPageRes<PayWayDto>.Pages(data);
        }

        /// <summary>
        /// 支付接口参数配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("配置费率")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_ADD, PermCode.MGR.ENT_AGENT_PAY_CONFIG_ADD, PermCode.MGR.ENT_MCH_PAY_CONFIG_ADD)]
        public ApiRes SaveOrUpdate(PayRateConfigSaveDto dto)
        {
            _payRateConfigService.SaveOrUpdate(dto);
            return ApiRes.Ok();
        }
    }
}
