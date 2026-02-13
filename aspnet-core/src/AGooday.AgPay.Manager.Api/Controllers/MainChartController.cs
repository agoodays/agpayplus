using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Base.Api.Attributes;
using AGooday.AgPay.Base.Api.Authorization;
using AGooday.AgPay.Base.Api.Controllers;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    /// <summary>
    /// 首页统计类
    /// </summary>
    [Route("api/mainChart")]
    [ApiController, Authorize, NoLog]
    public class MainChartController : CommonController
    {
        private readonly IPayOrderService _payOrderService;

        public MainChartController(ILogger<MainChartController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IPayOrderService payOrderService)
            : base(logger, cacheService, authService)
        {
            _payOrderService = payOrderService;
        }

        /// <summary>
        /// 今日/昨日交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payDayCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_DAY_COUNT)]
        public async Task<ApiRes> PayDayCountAsync(string queryDateRange)
        {
            return ApiRes.Ok(await _payOrderService.MainPagePayDayCountAsync(null, null, queryDateRange));
        }

        /// <summary>
        /// 趋势图统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTrendCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_TREND_COUNT)]
        public async Task<ApiRes> PayTrendCountAsync(int recentDay)
        {
            return ApiRes.Ok(await _payOrderService.MainPagePayTrendCountAsync(null, null, recentDay));
        }

        /// <summary>
        /// 服务商/代理商/商户统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("isvAndMchCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_ISV_MCH_COUNT)]
        public async Task<ApiRes> IsvAndMchCountAsync()
        {
            return ApiRes.Ok(await _payOrderService.MainPageIsvAndMchCountAsync(null, null));
        }

        /// <summary>
        /// 交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_COUNT)]
        public async Task<ApiRes> PayCountAsync(string queryDateRange)
        {
            return ApiRes.Ok(await _payOrderService.MainPagePayCountAsync(null, null, queryDateRange));
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_TYPE_COUNT)]
        public async Task<ApiRes> PayTypeCountAsync(string queryDateRange)
        {
            return ApiRes.Ok(await _payOrderService.MainPagePayTypeCountAsync(null, null, queryDateRange));
        }
    }
}
