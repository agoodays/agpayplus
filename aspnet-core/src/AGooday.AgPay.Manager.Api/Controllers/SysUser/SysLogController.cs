using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    [Route("/api/sysLog")]
    [ApiController]
    public class SysLogController : ControllerBase
    {
        private readonly ILogger<SysLogController> _logger;
        private readonly ISysLogService _sysLogService;

        public SysLogController(ILogger<SysLogController> logger, ISysLogService sysLogService)
        {
            _logger = logger;
            _sysLogService = sysLogService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List([FromBody] SysLogDto dto, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _sysLogService.GetPaginatedData(dto, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        [HttpDelete]
        [Route("delete/{sysLogId}")]
        public ApiRes Delete(long sysLogId)
        {
            _sysLogService.Remove(sysLogId);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{isvNo}")]
        public ApiRes Update(SysLogDto dto)
        {
            _sysLogService.Update(dto);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{sysLogId}")]
        public ApiRes Detail(long sysLogId)
        {
            var sysLog = _sysLogService.GetById(sysLogId);
            if (sysLog == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysLog);
        }
    }
}
