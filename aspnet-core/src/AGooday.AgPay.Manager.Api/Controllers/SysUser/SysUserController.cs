using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AGooday.AgPay.Manager.Api.Controllers.SysUser
{
    /// <summary>
    /// 操作员列表
    /// </summary>
    [Route("api/sysUsers")]
    [ApiController, Authorize]
    public class SysUserController : CommonController
    {
        private readonly ILogger<SysUserController> _logger;
        private readonly ISysUserService _sysUserService;
        private readonly ISysUserLoginAttemptService _sysUserLoginAttemptService;
        private readonly IMemoryCache _cache;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        public SysUserController(ILogger<SysUserController> logger, IMemoryCache cache, INotificationHandler<DomainNotification> notifications, RedisUtil client,
            ISysUserService sysUserService,
            ISysUserLoginAttemptService sysUserLoginAttemptService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _sysUserService = sysUserService;
            _sysUserLoginAttemptService = sysUserLoginAttemptService;
            _cache = cache;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 操作员信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_USER_LIST)]
        //public ApiRes List([FromQuery] SysUserQueryDto dto)
        //{
        //    var data = _sysUserService.GetPaginatedData(dto, GetCurrentUserId());
        //    return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        //}
        public async Task<ApiPageRes<SysUserListDto>> List([FromQuery] SysUserQueryDto dto)
        {
            long? currentUserId = null;//GetCurrentUserId();
            var data = await _sysUserService.GetPaginatedDataAsync(dto, currentUserId);
            return ApiPageRes<SysUserListDto>.Pages(data);
        }

        /// <summary>
        /// 添加操作员信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("添加操作员信息")]
        [PermissionAuth(PermCode.MGR.ENT_UR_USER_ADD)]
        public async Task<ApiRes> AddAsync(SysUserCreateDto dto)
        {
            //_cache.Remove("ErrorData");
            dto.SysType = string.IsNullOrWhiteSpace(dto.SysType) ? CS.SYS_TYPE.MGR : dto.SysType;
            dto.BelongInfoId = CS.BASE_BELONG_INFO_ID.MGR;
            dto.CreatedAt = DateTime.Now;
            dto.UpdatedAt = DateTime.Now;
            await _sysUserService.CreateAsync(dto);
            //var errorData = _cache.Get("ErrorData");
            //if (errorData == null)
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 删除操作员
        /// </summary>
        /// <param name="recordId">系统用户ID</param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除操作员")]
        [PermissionAuth(PermCode.MGR.ENT_UR_USER_DELETE)]
        public async Task<ApiRes> DeleteAsync(long recordId)
        {
            var currentUserId = 0;
            await _sysUserService.RemoveAsync(recordId, currentUserId, string.Empty);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
            {
                //如果用户被删除，需要更新redis数据
                RefAuthentication(new List<long> { recordId });
                return ApiRes.Ok();
            }
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 更新操作员信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("修改操作员信息")]
        [PermissionAuth(PermCode.MGR.ENT_UR_USER_EDIT)]
        public async Task<ApiRes> UpdateAsync(long recordId, SysUserModifyDto dto)
        {
            //dto.SysType = CS.SYS_TYPE.MGR;
            if (!dto.SysUserId.HasValue || dto.SysUserId.Value <= 0)
            {
                var sysUser = _sysUserService.GetByKeyAsNoTracking(recordId);
                sysUser.State = dto.State.Value;
                CopyUtil.CopyProperties(sysUser, dto);
            }
            await _sysUserService.ModifyAsync(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
            {
                if (dto.ResetPass.HasValue && dto.ResetPass.Value)
                {
                    // 删除用户redis缓存信息
                    DelAuthentication(new List<long> { dto.SysUserId.Value });
                }
                if (dto.State.HasValue && dto.State.Value.Equals(CS.PUB_DISABLE))
                {
                    //如果用户被禁用，需要更新redis数据
                    RefAuthentication(new List<long> { dto.SysUserId.Value });
                }
                return ApiRes.Ok();
            }
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 查看操作员信息
        /// </summary>
        /// <param name="recordId">系统用户ID</param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_UR_USER_EDIT)]
        //public ApiRes Detail(long recordId)
        //{
        //    var sysUser = _sysUserService.GetById(recordId);
        //    if (sysUser == null)
        //    {
        //        return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
        //    }
        //    return ApiRes.Ok(sysUser);
        //}
        public async Task<ApiRes> DetailAsync(long recordId)
        {
            var sysUser = await _sysUserService.GetByIdAsync(recordId);
            if (sysUser == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysUser);
        }

        /// <summary>
        /// 解除登录限制
        /// </summary>
        /// <param name="recordId">系统用户ID</param>
        /// <returns></returns>
        [HttpDelete, Route("loginLimit/{recordId}"), MethodLog("解除登录限制")]
        [PermissionAuth(PermCode.MGR.ENT_UR_USER_LOGIN_LIMIT_DELETE)]
        public async Task<ApiRes> RelieveLoginLimitAsync(long recordId)
        {
            await _sysUserLoginAttemptService.ClearFailedLoginAttemptsAsync(recordId);
            return ApiRes.Ok();
        }
    }
}