using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    /// <summary>
    /// 首页统计类
    /// </summary>
    [Route("api/mainChart")]
    [ApiController, Authorize, NoLog]
    public class MainChartController : ControllerBase
    {
        private readonly ILogger<MainChartController> _logger;
        private readonly IPayOrderService _payOrderService;

        public MainChartController(ILogger<MainChartController> logger, IPayOrderService payOrderService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
        }

        /// <summary>
        /// 周交易总金额
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payAmountWeek")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_AMOUNT_WEEK)]
        public ApiRes PayAmountWeek()
        {
            return ApiRes.Ok(_payOrderService.MainPageWeekCount(null, null));
        }

        /// <summary>
        /// 商户总数量、服务商总数量、总交易金额、总交易笔数
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("numCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_NUMBER_COUNT)]
        public ApiRes NumCount()
        {
            return ApiRes.Ok(_payOrderService.MainPageNumCount(null, null));
        }

        /// <summary>
        /// 今日/昨日交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payDayCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_DAY_COUNT)]
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
            return ApiRes.Ok(_payOrderService.MainPagePayDayCount(null, null, day));
        }

        /// <summary>
        /// 趋势图统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTrendCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_TREND_COUNT)]
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
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_ISV_MCH_COUNT)]
        public ApiRes IsvAndMchCount()
        {
            return ApiRes.Ok(_payOrderService.MainPageIsvAndMchCount(null, null));
        }

        /// <summary>
        /// 交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_COUNT)]
        public ApiRes PayCount(string queryDateRange)
        {
            DateUtil.GetQueryDateRange(queryDateRange, out string createdStart, out string createdEnd);
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
            //return ApiRes.Ok(_payOrderService.MainPagePayCount(null, null, createdStart, createdEnd));
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_TYPE_COUNT)]
        public ApiRes PayWayCount(string queryDateRange)
        {
            DateUtil.GetQueryDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
            return ApiRes.Ok(_payOrderService.MainPagePayTypeCount(null, null, createdStart, createdEnd));
        }
    }
}
