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

namespace AGooday.AgPay.Agent.Api.Controllers.SysArticle
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
        [PermissionAuth(PermCode.AGENT.ENT_ARTICLE_NOTICEINFO)]
        public async Task<ApiPageRes<SysArticleDto>> ListAsync([FromQuery] SysArticleQueryDto dto)
        {
            dto.BindDateRange();
            dto.ArticleRange = "AGENT";
            var data = await _sysArticleService.GetPaginatedDataAsync(dto);
            return ApiPageRes<SysArticleDto>.Pages(data);
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_ARTICLE_NOTICEINFO)]
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
