using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("/api/sysRoleEntRela")]
    [ApiController]
    public class SysRoleEntRelaController : ControllerBase
    {
        private readonly ILogger<SysRoleEntRelaController> _logger;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;

        public SysRoleEntRelaController(ILogger<SysRoleEntRelaController> logger, ISysRoleEntRelaService sysRoleEntRelaService)
        {
            _logger = logger;
            _sysRoleEntRelaService = sysRoleEntRelaService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List([FromBody] SysRoleEntRelaDto dto, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _sysRoleEntRelaService.GetPaginatedData(dto, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }
    }
}
