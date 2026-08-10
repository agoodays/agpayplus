using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Base.Api.Attributes;
using AGooday.AgPay.Base.Api.Authorization;
using AGooday.AgPay.Base.Api.Controllers;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.Third.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户进件管理
    ///
    /// 依赖分离（参考 PayOrderProcessService 模式）：
    ///   IMchApplyService        → Application 层纯 DB CRUD
    ///   MchApplymentService     → Components.Third 层业务编排 + 通道调用（MQ 异步回写）
    /// </summary>
    [Route("api/mchApply")]
    [ApiController, Authorize]
    public class MchApplyController : CommonController
    {
        private readonly IMchApplyService _mchApplyService;
        private readonly MchApplymentService _mchApplymentService;

        public MchApplyController(ILogger<MchApplyController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMchApplyService mchApplyService,
            MchApplymentService mchApplymentService)
            : base(logger, cacheService, authService)
        {
            _mchApplyService = mchApplyService;
            _mchApplymentService = mchApplymentService;
        }

        /// <summary>进件单列表</summary>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_LIST)]
        public async Task<ApiPageRes<MchApplyDto>> ListAsync([FromQuery] MchApplyQueryDto dto)
        {
            var data = await _mchApplyService.GetPaginatedDataAsync(dto);
            return ApiPageRes<MchApplyDto>.Pages(data);
        }

        /// <summary>进件单详情</summary>
        [HttpGet, Route("{applyId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_VIEW, PermCode.MGR.ENT_MCH_APPLY_EDIT)]
        public async Task<ApiRes> GetByIdAsync(string applyId)
        {
            var data = await _mchApplyService.GetByIdAsync(applyId);
            return ApiRes.Ok(data);
        }

        /// <summary>保存草稿</summary>
        [HttpPost, Route(""), MethodLog("保存商户进件草稿")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_ADD)]
        public async Task<ApiRes> SaveDraftAsync([FromBody] MchApplySaveDto dto)
        {
            var userId = await GetCurrentUserIdAsync();
            var userName = (await GetCurrentUserAsync()).SysUser.LoginUsername;
            var data = await _mchApplyService.SaveDraftAsync(dto, userId, userName);
            return ApiRes.Ok(data);
        }

        /// <summary>提交进件</summary>
        [HttpPost, Route("{applyId}/submit"), MethodLog("提交商户进件")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_SUBMIT)]
        public async Task<ApiRes> SubmitAsync(string applyId)
        {
            var userId = await GetCurrentUserIdAsync();
            var userName = (await GetCurrentUserAsync()).SysUser.LoginUsername;
            var data = await _mchApplyService.SubmitAsync(applyId, userId, userName);
            return ApiRes.Ok(data);
        }

        /// <summary>预审通过/拒绝</summary>
        [HttpPost, Route("{applyId}/audit"), MethodLog("商户进件预审")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_AUDIT)]
        public async Task<ApiRes> AuditAsync(string applyId, [FromBody] MchApplyAuditDto dto)
        {
            dto.ApplyId = applyId;
            var userId = await GetCurrentUserIdAsync();
            var userName = (await GetCurrentUserAsync()).SysUser.LoginUsername;
            var data = await _mchApplymentService.AuditAsync(dto, userId, userName);
            return ApiRes.Ok(data);
        }

        /// <summary>查询渠道进件结果</summary>
        [HttpPost, Route("{applyId}/queryChannel"), MethodLog("查询商户进件渠道结果")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_QUERY_CHANNEL)]
        public async Task<ApiRes> QueryChannelAsync(string applyId)
        {
            var data = await _mchApplymentService.QueryChannelResultAsync(applyId);
            return ApiRes.Ok(data);
        }

        /// <summary>获取签约链接</summary>
        [HttpPost, Route("{applyId}/signUrl"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_SIGN_URL)]
        public async Task<ApiRes> GetSignUrlAsync(string applyId)
        {
            var data = await _mchApplymentService.GetSignUrlAsync(applyId);
            return ApiRes.Ok(data);
        }

        /// <summary>小额打款验证</summary>
        [HttpPost, Route("{applyId}/verify"), MethodLog("商户进件小额打款验证")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_VERIFY)]
        public async Task<ApiRes> VerifyAsync(string applyId, [FromBody] MchApplyVerifyDto dto)
        {
            dto.ApplyId = applyId;
            var userId = await GetCurrentUserIdAsync();
            var userName = (await GetCurrentUserAsync()).SysUser.LoginUsername;
            var data = await _mchApplymentService.VerifyAsync(dto, userId, userName);
            return ApiRes.Ok(data);
        }

        /// <summary>删除草稿</summary>
        [HttpDelete, Route("{applyId}"), MethodLog("删除商户进件草稿")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APPLY_DEL)]
        public async Task<ApiRes> RemoveAsync(string applyId)
        {
            await _mchApplyService.RemoveDraftAsync(applyId);
            return ApiRes.Ok();
        }
    }
}
