using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    [Route("/api/sysUserRoleRela")]
    [ApiController]
    public class SysUserRoleRelaController : CommonController
    {
        private readonly ILogger<SysUserRoleRelaController> _logger;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysUserRoleRelaController(ILogger<SysUserRoleRelaController> logger, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _sysUserRoleRelaService = sysUserRoleRelaService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List([FromBody] SysUserRoleRelaDto dto, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _sysUserRoleRelaService.GetPaginatedData(dto, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        [HttpPost]
        [Route("relas/{sysUserId}")]
        public ApiRes Relas(long sysUserId)
        {
            RefAuthentication(new List<long> { sysUserId });
            return ApiRes.Ok();
        }
    }
}
