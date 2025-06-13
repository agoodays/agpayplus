using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.SysArticle
{
    /// <summary>
    /// 文章
    /// </summary>
    [Route("api/sysArticles")]
    [ApiController, Authorize]
    public class SysArticleController : CommonController
    {
        private readonly ISysArticleService _sysArticleService;

        public SysArticleController(ILogger<SysArticleController> logger,
            ICacheService cacheService,
            IAuthService authService,
            ISysArticleService sysArticleService)
            : base(logger, cacheService, authService)
        {
            _sysArticleService = sysArticleService;
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_NOTICE_LIST)]
        public async Task<ApiPageRes<SysArticleDto>> ListAsync([FromQuery] SysArticleQueryDto dto)
        {
            dto.BindDateRange();
            var data = await _sysArticleService.GetPaginatedDataAsync(dto);
            return ApiPageRes<SysArticleDto>.Pages(data);
        }

        /// <summary>
        /// 新建文章
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建文章")]
        [PermissionAuth(PermCode.MGR.ENT_NOTICE_ADD)]
        public async Task<ApiRes> AddAsync(SysArticleDto dto)
        {
            var sysUser = (await GetCurrentUserAsync()).SysUser;
            dto.ArticleType = (byte)ArticleType.NOTICE;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            var result = await _sysArticleService.AddAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除文章")]
        [PermissionAuth(PermCode.MGR.ENT_NOTICE_DEL)]
        public async Task<ApiRes> DeleteAsync(long recordId)
        {
            var sysArticle = await _sysArticleService.GetByIdAsync(recordId);
            if (sysArticle == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            await _sysArticleService.RemoveAsync(recordId);

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新文章信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新文章信息")]
        [PermissionAuth(PermCode.MGR.ENT_NOTICE_EDIT)]
        public async Task<ApiRes> UpdateAsync(long recordId, SysArticleDto dto)
        {
            var result = await _sysArticleService.UpdateAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }

            return ApiRes.Ok();
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_NOTICE_VIEW, PermCode.MGR.ENT_NOTICE_EDIT)]
        public async Task<ApiRes> DetailAsync(long recordId)
        {
            var sysArticle = await _sysArticleService.GetByIdAsNoTrackingAsync(recordId);
            if (sysArticle == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysArticle);
        }
    }
}
