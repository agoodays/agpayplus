﻿using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers
{
    /// <summary>
    /// 主页数据
    /// </summary>
    [Route("api/mainChart")]
    [ApiController, NoLog]
    public class MainChartController : CommonController
    {
        private readonly IPayOrderService _payOrderService;
        private readonly ISysUserService _sysUserService;
        private readonly IMchInfoService _mchInfoService;

        public MainChartController(ILogger<MainChartController> logger,
            IPayOrderService payOrderService,
            IMchInfoService mchInfoService,
            ISysUserService sysUserService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _payOrderService = payOrderService;
            _mchInfoService = mchInfoService;
            _sysUserService = sysUserService;
        }

        /// <summary>
        /// 商户基本信息、用户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_INFO)]
        public async Task<ApiRes> MchInfoAsync()
        {
            var sysUser = await _sysUserService.GetByIdAsync(GetCurrentUser().SysUser.SysUserId);
            var mchInfo = await _mchInfoService.GetByIdAsync(GetCurrentMchNo());
            //var jobj = JObject.FromObject(mchInfo);
            //jobj.Add("loginUsername", sysUser.LoginUsername);
            //jobj.Add("realname", sysUser.Realname);
            mchInfo.AddExt("loginUsername", sysUser.LoginUsername);
            mchInfo.AddExt("realname", sysUser.Realname);
            return ApiRes.Ok(mchInfo);
        }

        /// <summary>
        /// 今日/昨日交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payDayCount")]
        [PermissionAuth(PermCode.MCH.ENT_C_MAIN_PAY_DAY_COUNT)]
        public ApiRes PayDayCount(string queryDateRange)
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
            return ApiRes.Ok(_payOrderService.MainPagePayDayCount(GetCurrentMchNo(), null, day));
        }

        /// <summary>
        /// 趋势图统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTrendCount")]
        [PermissionAuth(PermCode.MCH.ENT_C_MAIN_PAY_TREND_COUNT)]
        public ApiRes PayTrendCount(int recentDay)
        {
            return ApiRes.Ok(_payOrderService.MainPagePayTrendCount(GetCurrentMchNo(), null, recentDay));
        }

        /// <summary>
        /// 交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payCount")]
        [PermissionAuth(PermCode.MCH.ENT_C_MAIN_PAY_COUNT)]
        public ApiRes PayCount(string queryDateRange)
        {
            DateUtil.GetQueryDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return ApiRes.Ok(_payOrderService.MainPagePayCount(GetCurrentMchNo(), null, createdStart, createdEnd));
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        [PermissionAuth(PermCode.MCH.ENT_C_MAIN_PAY_TYPE_COUNT)]
        public ApiRes PayTypeCount(string queryDateRange)
        {
            DateUtil.GetQueryDateRange(queryDateRange, out string createdStart, out string createdEnd);
            if (string.IsNullOrWhiteSpace(createdStart) && string.IsNullOrWhiteSpace(createdEnd))
            {
                createdStart = DateTime.Today.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return ApiRes.Ok(_payOrderService.MainPagePayTypeCount(GetCurrentMchNo(), null, createdStart, createdEnd));
        }
    }
}
