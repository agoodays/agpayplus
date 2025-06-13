using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    /// <summary>
    /// 系统日志记录类
    /// </summary>
    [Route("api/sysLog")]
    [ApiController, Authorize]
    public class SysLogController : CommonController
    {
        private readonly ISysLogService _sysLogService;

        public SysLogController(ILogger<SysLogController> logger,
            ICacheService cacheService,
            IAuthService authService,
            ISysLogService sysLogService)
            : base(logger, cacheService, authService)
        {
            _sysLogService = sysLogService;
        }

        /// <summary>
        /// 日志记录列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_LOG_LIST)]
        public async Task<ApiPageRes<SysLogDto>> ListAsync([FromQuery] SysLogQueryDto dto)
        {
            dto.BindDateRange();
            var data = await _sysLogService.GetPaginatedDataAsync(dto);
            return ApiPageRes<SysLogDto>.Pages(data);
        }

        /// <summary>
        /// 查看日志信息
        /// </summary>
        /// <param name="sysLogId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{selectedIds}"), MethodLog("删除日志信息")]
        [PermissionAuth(PermCode.MGR.ENT_SYS_LOG_DEL)]
        public async Task<ApiRes> DeleteAsync(string selectedIds)
        {
            var ids = selectedIds.Split(",").Select(s => Convert.ToInt64(s)).ToList();
            var result = await _sysLogService.RemoveByIdsAsync(ids);
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
        [HttpGet, Route("{sysLogId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_SYS_LOG_VIEW)]
        public async Task<ApiRes> DetailAsync(long sysLogId)
        {
            var sysLog = await _sysLogService.GetByIdAsNoTrackingAsync(sysLogId);
            if (sysLog == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysLog);
        }
    }
}
