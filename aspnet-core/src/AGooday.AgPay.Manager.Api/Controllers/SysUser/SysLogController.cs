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
    /// <summary>
    /// 系统日志记录类
    /// </summary>
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

        /// <summary>
        /// 日志记录列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ApiRes List([FromQuery] SysLogQueryDto dto)
        {
            var data = _sysLogService.GetPaginatedData(dto);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        /// <summary>
        /// 查看日志信息
        /// </summary>
        /// <param name="sysLogId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{selectedIds}")]
        public ApiRes Delete(string selectedIds)
        {
            var ids = selectedIds.Split(",").Select(s => Convert.ToInt64(s)).ToList();
            var result = _sysLogService.RemoveByIds(ids);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_DELETE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看日志信息
        /// </summary>
        /// <param name="sysLogId"></param>
        /// <returns></returns>
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
