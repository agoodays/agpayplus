using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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
            var jobj = JObject.FromObject(agentInfo);
            jobj.Add("loginUsername", sysUser.LoginUsername);
            jobj.Add("realname", sysUser.Realname);
            return ApiRes.Ok(jobj);
        }

        /// <summary>
        /// 周交易总金额
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payAmountWeek")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_AMOUNT_WEEK)]
        public ApiRes PayAmountWeek()
        {
            return ApiRes.Ok(_payOrderService.MainPageWeekCount(null, GetCurrentAgentNo()));
        }

        /// <summary>
        /// 商户总数量、服务商总数量、总交易金额、总交易笔数
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("numCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_NUMBER_COUNT)]
        public ApiRes NumCount()
        {
            return ApiRes.Ok(_payOrderService.MainPageNumCount(null, GetCurrentAgentNo()));
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
                case "yesterday":
                    day?.AddDays(-1); break;
                case "today":
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
            List<string> dateList = new List<string>();
            List<string> payAmountList = new List<string>();
            for (int i = recentDay - 1; i >= 0; i--)
            {
                dateList.Add(DateTime.Now.AddDays(-i).ToString("MM-dd"));
                payAmountList.Add((Random.Shared.Next(10000, 1000000) / 100.00).ToString("0.00"));
            }
            return ApiRes.Ok(new { dateList, payAmountList });
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
            GetDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
            List<string> resDateArr = new List<string>();
            List<string> resPayAmountArr = new List<string>();
            List<string> resPayCountArr = new List<string>();
            List<string> resRefAmountArr = new List<string>();

            for (DateTime dt = Convert.ToDateTime(createdStart); dt < Convert.ToDateTime(createdEnd); dt = dt.AddDays(1))
            {
                resDateArr.Add(dt.ToString("yyyy-MM-dd"));
                resPayAmountArr.Add((Random.Shared.Next(10000, 1000000) / 100.00).ToString("0.00"));
                resPayCountArr.Add(Random.Shared.Next(100, 1000).ToString());
                resRefAmountArr.Add((Random.Shared.Next(5000, 500000) / 100.00).ToString("0.00"));
            }
            return ApiRes.Ok(new { resDateArr, resPayAmountArr, resPayCountArr, resRefAmountArr });
            //return ApiRes.Ok(_payOrderService.MainPagePayCount(null, GetCurrentAgentNo(), createdStart, createdEnd));
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        [PermissionAuth(PermCode.AGENT.ENT_C_MAIN_PAY_TYPE_COUNT)]
        public ApiRes PayWayCount(string queryDateRange)
        {
            GetDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
            return ApiRes.Ok(_payOrderService.MainPagePayTypeCount(null, GetCurrentAgentNo(), createdStart, createdEnd));
        }

        private static void GetDateRange(string queryDateRange, out string createdStart, out string createdEnd)
        {
            createdStart = null; createdEnd = null;
            queryDateRange = queryDateRange ?? string.Empty;
            if (queryDateRange.Equals("today"))
            {
                createdStart = DateTime.Today.ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
            if (queryDateRange.Equals("yesterday"))
            {
                createdStart = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.ToString("yyyy-MM-dd");
            }
            if (queryDateRange.Contains("near2now"))
            {
                int day = Convert.ToInt32(queryDateRange.Split("_")[1]);
                createdStart = DateTime.Today.AddDays(-(day - 1)).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
            if (queryDateRange.Contains("customDateTime"))
            {
                createdStart = Convert.ToDateTime(queryDateRange.Split("_")[1]).ToString("yyyy-MM-dd");
                createdEnd = Convert.ToDateTime(queryDateRange.Split("_")[2]).AddDays(1).ToString("yyyy-MM-dd");
            }
        }
    }
}
