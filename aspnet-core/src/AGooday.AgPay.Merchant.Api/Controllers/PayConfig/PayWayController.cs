﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayConfig
{
    /// <summary>
    /// 支付方式管理类
    /// </summary>
    [Route("api/payWays")]
    [ApiController, Authorize, NoLog]
    public class PayWayController : CommonController
    {
        private readonly IPayWayService _payWayService;

        public PayWayController(ILogger<PayWayController> logger,
            IPayWayService payWayService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _payWayService = payWayService;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MCH.ENT_PAY_ORDER_SEARCH_PAY_WAY)]
        public ApiPageRes<PayWayDto> List([FromQuery] PayWayQueryDto dto)
        {
            var data = _payWayService.GetPaginatedData<PayWayDto>(dto);
            return ApiPageRes<PayWayDto>.Pages(data);
        }
    }
}
