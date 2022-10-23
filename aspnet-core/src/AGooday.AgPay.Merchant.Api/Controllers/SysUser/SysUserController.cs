using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.InteropServices;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Merchant.Api.Authorization;

namespace AGooday.AgPay.Merchant.Api.Controllers.SysUser
{
    /// <summary>
    /// 操作员列表
    /// </summary>
    [ApiController, Authorize]
    [Route("api/sysUsers")]
    public class SysUserController : CommonController
    {
        private readonly ILogger<SysUserController> _logger;
        private readonly ISysUserService _sysUserService;
        private IMemoryCache _cache;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        public SysUserController(ILogger<SysUserController> logger, IMemoryCache cache, INotificationHandler<DomainNotification> notifications, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _sysUserService = sysUserService;
            _cache = cache;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 操作员信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MCH.ENT_UR_USER_LIST)]
        public ApiRes List([FromQuery] SysUserQueryDto dto)
        {
            dto.SysType = CS.SYS_TYPE.MCH;
            var data = _sysUserService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 添加操作员信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("")]
        [PermissionAuth(PermCode.MCH.ENT_UR_USER_ADD)]
        public ApiRes Add(SysUserCreateDto dto)
        {
            //_cache.Remove("ErrorData");
            dto.IsAdmin = CS.NO;
            dto.SysType = CS.SYS_TYPE.MCH;
            dto.BelongInfoId = GetCurrentUser().User.BelongInfoId;
            _sysUserService.Create(dto);
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
        [HttpDelete, Route("{recordId}")]
        [PermissionAuth(PermCode.MCH.ENT_UR_USER_DELETE)]
        public ApiRes Delete(long recordId)
        {
            var currentUserId = 0;
            //判断是否删除商户默认超管
            var dbRecord = _sysUserService.GetById(recordId, GetCurrentUser().User.BelongInfoId);
            if (dbRecord != null && dbRecord.SysType == CS.SYS_TYPE.MCH && dbRecord.IsAdmin == CS.YES)
            {
                return ApiRes.CustomFail("系统不允许删除商户默认用户！");
            }
            _sysUserService.Remove(recordId, currentUserId, CS.SYS_TYPE.MCH);
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
        [HttpPut, Route("{recordId}")]
        [PermissionAuth(PermCode.MCH.ENT_UR_USER_EDIT)]
        public ApiRes Update(long recordId, SysUserModifyDto dto)
        {
            dto.SysType = CS.SYS_TYPE.MCH;
            var dbRecord = _sysUserService.GetById(dto.SysUserId, GetCurrentUser().User.BelongInfoId);
            if (dbRecord == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            _sysUserService.Modify(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
            {
                if (dto.ResetPass)
                {
                    // 删除用户redis缓存信息
                    DelAuthentication(new List<long> { dto.SysUserId });
                }
                if (dto.State.Equals(CS.PUB_DISABLE))
                {
                    //如果用户被禁用，需要更新redis数据
                    RefAuthentication(new List<long> { dto.SysUserId });
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
        [HttpGet, Route("{recordId}")]
        [PermissionAuth(PermCode.MCH.ENT_UR_USER_EDIT)]
        public ApiRes Detail(long recordId)
        {
            var sysUser = _sysUserService.GetById(recordId);
            if (sysUser == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (sysUser.BelongInfoId.Equals(GetCurrentUser().User.BelongInfoId))
            {
                return ApiRes.Fail(ApiCode.SYS_PERMISSION_ERROR);
            }
            return ApiRes.Ok(sysUser);
        }
    }
}