using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
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
    public class StatisticController : CommonController
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly IStatisticService _statisticService;

        public StatisticController(ILogger<StatisticController> logger,
            IStatisticService statisticService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _statisticService = statisticService;
        }

        /// <summary>
        /// 统计列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_STATISTIC)]
        public ApiPageRes<StatisticResultDto> Statistics([FromQuery] StatisticQueryDto dto)
        {
            ChickAuth(dto.Method);
            dto.BindDateRange();
            var result = _statisticService.Statistics(dto);
            return ApiPageRes<StatisticResultDto>.Pages(result);
        }

        /// <summary>
        /// 统计合计
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("total"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_STATISTIC)]
        public ApiRes Total([FromQuery] StatisticQueryDto dto)
        {
            ChickAuth(dto.Method);
            dto.BindDateRange();
            var statistics = _statisticService.Total(dto);
            return ApiRes.Ok(statistics);
        }

        private void ChickAuth(string method)
        {
            if (method.Equals("transaction", StringComparison.OrdinalIgnoreCase) && !GetCurrentUser().Authorities.Contains(PermCode.MGR.ENT_STATISTIC_TRANSACTION))
            {
                throw new BizException("当前用户未分配该菜单权限！");
            }
            if (method.Equals("mch", StringComparison.OrdinalIgnoreCase) && !GetCurrentUser().Authorities.Contains(PermCode.MGR.ENT_STATISTIC_MCH))
            {
                throw new BizException("当前用户未分配该菜单权限！");
            }
        }
    }
}
