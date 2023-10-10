using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
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
        private readonly ILogger<SysArticleController> _logger;
        private readonly ISysArticleService _sysArticleService;

        public SysArticleController(IMQSender mqSender, ILogger<SysArticleController> logger,
            ISysArticleService sysArticleService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
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
        [PermissionAuth(PermCode.MGR.ENT_NOTICE_LIST)]
        public ApiPageRes<SysArticleDto> List([FromQuery] SysArticleQueryDto dto)
        {
            dto.BindDateRange();
            var data = _sysArticleService.GetPaginatedData(dto);
            return ApiPageRes<SysArticleDto>.Pages(data);
        }

        /// <summary>
        /// 新建文章
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建文章")]
        [PermissionAuth(PermCode.MGR.ENT_NOTICE_ADD)]
        public ApiRes Add(SysArticleDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.ArticleType = (byte)ArticleType.NOTICE;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            var result = _sysArticleService.Add(dto);
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
        public ApiRes Delete(long recordId)
        {
            var sysArticle = _sysArticleService.GetById(recordId);
            _sysArticleService.Remove(recordId);

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新文章信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新文章信息")]
        [PermissionAuth(PermCode.MGR.ENT_NOTICE_EDIT)]
        public ApiRes Update(long recordId, SysArticleDto dto)
        {
            var result = _sysArticleService.Update(dto);
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
