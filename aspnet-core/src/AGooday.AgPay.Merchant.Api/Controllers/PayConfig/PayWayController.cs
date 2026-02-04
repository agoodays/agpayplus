using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Base.Api.Attributes;
using AGooday.AgPay.Base.Api.Authorization;
using AGooday.AgPay.Base.Api.Controllers;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
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
            ICacheService cacheService,
            IAuthService authService,
            IPayWayService payWayService)
            : base(logger, cacheService, authService)
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
        public async Task<ApiPageRes<PayWayDto>> ListAsync([FromQuery] PayWayQueryDto dto)
        {
            var data = await _payWayService.GetPaginatedDataAsync<PayWayDto>(dto);
            return ApiPageRes<PayWayDto>.Pages(data);
        }
    }
}
