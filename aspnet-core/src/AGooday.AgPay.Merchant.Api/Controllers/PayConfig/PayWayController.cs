using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Merchant.Api.Authorization;
using AGooday.AgPay.Merchant.Api.Attributes;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayConfig
{
    /// <summary>
    /// 支付方式管理类
    /// </summary>
    [Route("/api/payWays")]
    [ApiController, Authorize, NoLog]
    public class PayWayController : ControllerBase
    {
        private readonly ILogger<PayWayController> _logger;
        private readonly IPayWayService _payWayService;

        public PayWayController(ILogger<PayWayController> logger,
            IPayWayService payWayService)
        {
            _logger = logger;
            _payWayService = payWayService;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet,Route("")]
        [PermissionAuth(PermCode.MCH.ENT_PAY_ORDER_SEARCH_PAY_WAY)]
        public ApiRes List([FromQuery] PayWayQueryDto dto)
        {
            var data = _payWayService.GetPaginatedData<PayWayDto>(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }
    }
}
