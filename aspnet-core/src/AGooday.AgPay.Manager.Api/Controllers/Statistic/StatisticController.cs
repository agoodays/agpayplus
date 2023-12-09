using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Statistic
{
    /// <summary>
    /// 数据统计
    /// </summary>
    [Route("/api/statistic")]
    [ApiController, Authorize]
    public class StatisticController : ControllerBase
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly IPayOrderService _payOrderService;

        public StatisticController(ILogger<StatisticController> logger,
            IPayOrderService payOrderService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
        }

        /// <summary>
        /// 统计列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_LIST)]
        public ApiPageRes<StatisticResultDto> List([FromQuery] StatisticQueryDto dto)
        {
            dto.BindDateRange();
            return ApiPageRes<StatisticResultDto>.Pages(null);
        }

        /// <summary>
        /// 统计合计
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("total"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_LIST)]
        public ApiRes Total([FromQuery] PayOrderQueryDto dto)
        {
            dto.BindDateRange();
            var statistics = _payOrderService.Statistics(dto);
            return ApiRes.Ok(statistics);
        }
    }
}
