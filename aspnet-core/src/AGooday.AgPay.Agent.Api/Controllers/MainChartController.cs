using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers
{
    /// <summary>
    /// 首页统计类
    /// </summary>
    [Route("api/mainChart")]
    [ApiController, Authorize, NoLog]
    public class MainChartController : CommonController
    {
        private readonly ILogger<MainChartController> _logger;
        private readonly IPayOrderService _payOrderService;
        private readonly ISysUserService _sysUserService;
        private readonly IAgentInfoService _agentInfoService;

        public MainChartController(ILogger<MainChartController> logger, RedisUtil client,
            IPayOrderService payOrderService,
            IAgentInfoService agentInfoService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
            _sysUserService = sysUserService;
            _agentInfoService = agentInfoService;
        }

        /// <summary>
        /// 代理商基本信息、用户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.AGENT.ENT_AGENT_CURRENT_INFO)]
        public ApiRes AgentInfo()
        {
            var sysUser = _sysUserService.GetById(GetCurrentUser().SysUser.SysUserId);
            var agentInfo = _agentInfoService.GetById(GetCurrentAgentNo());
            //var jobj = JObject.FromObject(agentInfo);
            //jobj.Add("loginUsername", sysUser.LoginUsername);
            //jobj.Add("realname", sysUser.Realname);
            agentInfo.AddExt("loginUsername", sysUser.LoginUsername);
            agentInfo.AddExt("realname", sysUser.Realname);
            return ApiRes.Ok(agentInfo);
        }

        /// <summary>
        /// 今日/昨日交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payDayCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_DAY_COUNT)]
        public ApiRes PayDayCount(string queryDateRange)
        {
            DateTime? day = DateTime.Today;
            switch (queryDateRange)
            {
                case DateUtil.YESTERDAY:
                    day?.AddDays(-1); break;
                case DateUtil.TODAY:
                default:
                    break;
            }
            return ApiRes.Ok(_payOrderService.MainPagePayDayCount(null, GetCurrentAgentNo(), day));
        }

        /// <summary>
        /// 趋势图统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTrendCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_TREND_COUNT)]
        public ApiRes PayTrendCount(int recentDay)
        {
            return ApiRes.Ok(_payOrderService.MainPagePayTrendCount(null, GetCurrentAgentNo(), recentDay));
        }

        /// <summary>
        /// 服务商/代理商/商户统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("isvAndMchCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_ISV_MCH_COUNT)]
        public ApiRes IsvAndMchCount()
        {
            return ApiRes.Ok(_payOrderService.MainPageIsvAndMchCount(null, GetCurrentAgentNo()));
        }

        /// <summary>
        /// 交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_COUNT)]
        public ApiRes PayCount(string queryDateRange)
        {
            DateUtil.GetQueryDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return ApiRes.Ok(_payOrderService.MainPagePayCount(null, GetCurrentAgentNo(), createdStart, createdEnd));
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_TYPE_COUNT)]
        public ApiRes PayTypeCount(string queryDateRange)
        {
            DateUtil.GetQueryDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return ApiRes.Ok(_payOrderService.MainPagePayTypeCount(null, GetCurrentAgentNo(), createdStart, createdEnd));
        }
    }
}
