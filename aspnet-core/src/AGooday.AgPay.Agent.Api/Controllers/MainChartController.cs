using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
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
        private readonly IAgentInfoService _agentInfoService;
        private readonly IPayOrderService _payOrderService;

        public MainChartController(ILogger<MainChartController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IAgentInfoService agentInfoService,
            IPayOrderService payOrderService)
            : base(logger, cacheService, authService)
        {
            _agentInfoService = agentInfoService;
            _payOrderService = payOrderService;
        }

        /// <summary>
        /// 代理商基本信息、用户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.AGENT.ENT_AGENT_CURRENT_INFO)]
        public async Task<ApiRes> AgentInfoAsync()
        {
            var sysUser = await _authService.GetUserByIdAsync(await GetCurrentUserIdAsync());
            var agentInfo = await _agentInfoService.GetByIdAsync(await GetCurrentAgentNoAsync());
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
        public async Task<ApiRes> PayDayCountAsync(string queryDateRange)
        {
            DateTime? day = DateTime.Today;
            switch (queryDateRange)
            {
                case DateUtil.YESTERDAY:
                    day = day?.AddDays(-1); break;
                case DateUtil.TODAY:
                default:
                    break;
            }
            return ApiRes.Ok(await _payOrderService.MainPagePayDayCountAsync(null, await GetCurrentAgentNoAsync(), day));
        }

        /// <summary>
        /// 趋势图统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTrendCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_TREND_COUNT)]
        public async Task<ApiRes> PayTrendCountAsync(int recentDay)
        {
            return ApiRes.Ok(await _payOrderService.MainPagePayTrendCountAsync(null, await GetCurrentAgentNoAsync(), recentDay));
        }

        /// <summary>
        /// 服务商/代理商/商户统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("isvAndMchCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_ISV_MCH_COUNT)]
        public async Task<ApiRes> IsvAndMchCountAsync()
        {
            return ApiRes.Ok(await _payOrderService.MainPageIsvAndMchCountAsync(null, await GetCurrentAgentNoAsync()));
        }

        /// <summary>
        /// 交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_COUNT)]
        public async Task<ApiRes> PayCountAsync(string queryDateRange)
        {
            DateUtil.GetQueryDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return ApiRes.Ok(await _payOrderService.MainPagePayCountAsync(null, await GetCurrentAgentNoAsync(), createdStart, createdEnd));
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_TYPE_COUNT)]
        public async Task<ApiRes> PayTypeCountAsync(string queryDateRange)
        {
            DateUtil.GetQueryDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return ApiRes.Ok(await _payOrderService.MainPagePayTypeCountAsync(null, await GetCurrentAgentNoAsync(), createdStart, createdEnd));
        }
    }
}
