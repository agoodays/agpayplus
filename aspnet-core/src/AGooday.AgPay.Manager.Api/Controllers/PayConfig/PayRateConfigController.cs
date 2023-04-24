using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
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
    [Route("/api/rateConfig")]
    [ApiController, Authorize]
    public class PayRateConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<PayInterfaceConfigController> _logger;
        private readonly IPayRateConfigService _payRateConfigService;
        private readonly IPayWayService _payWayService;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public PayRateConfigController(IMQSender mqSender,
            IPayRateConfigService payRateConfigService,
            IPayWayService payWayService,
            IPayInterfaceConfigService payIfConfigService,
            ILogger<PayInterfaceConfigController> logger,
            RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _payWayService = payWayService;
            _payIfConfigService = payIfConfigService;
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
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_LIST, PermCode.MGR.ENT_MCH_PAY_CONFIG_LIST)]
        public ApiRes List(string configMode, string infoId, string ifCode)
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
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_LIST, PermCode.MGR.ENT_MCH_PAY_CONFIG_LIST)]
        public ApiRes GetPayWaysByInfoId(string configMode, string infoId, string ifCode)
        {
            string infoType = string.Empty;
            switch (configMode)
            {
                case "mgrIsv":
                    infoType = CS.INFO_TYPE_ISV;
                    break;
                case "mgrAgent":
                case "agentSubagent":
                    infoType = CS.INFO_TYPE_AGENT;
                    break;
                case "mgrMch":
                case "agentMch":
                case "agentSelf":
                case "mchSelfApp1":
                case "mchSelfApp2":
                    infoType = CS.INFO_TYPE_MCH_APP;
                    break;
                default:
                    break;
            }
            var data = _payWayService.GetPaginatedData<PayWayDto>(new PayWayQueryDto());
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 支付接口参数配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("配置费率")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_ADD, PermCode.MGR.ENT_MCH_PAY_CONFIG_ADD)]
        public ApiRes SaveOrUpdate(PayRateConfigSaveDto dto)
        {
            _payRateConfigService.SaveOrUpdate(dto);
            return ApiRes.Ok();
        }
    }
}
