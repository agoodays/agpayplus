using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.SysArticle
{
    /// <summary>
    /// 文章
    /// </summary>
    [Route("api/sysArticles")]
    [ApiController, Authorize]
    public class SysArticleController : ControllerBase
    {
        private readonly ILogger<SysArticleController> _logger;
        private readonly ISysArticleService _sysArticleService;

        public SysArticleController(ILogger<SysArticleController> logger,
            ISysArticleService sysArticleService)
        {
            _logger = logger;
            _sysArticleService = sysArticleService;
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_ARTICLE_NOTICEINFO)]
        public ApiPageRes<SysArticleDto> List([FromQuery] SysArticleQueryDto dto)
        {
            dto.BindDateRange();
            dto.ArticleRange = "MCH";
            var data = _sysArticleService.GetPaginatedData(dto);
            return ApiPageRes<SysArticleDto>.Pages(data);
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_ARTICLE_NOTICEINFO)]
        public ApiRes Detail(long recordId)
        {
            var sysArticle = _sysArticleService.GetById(recordId);
            if (sysArticle == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysArticle);
        }
    }
}
