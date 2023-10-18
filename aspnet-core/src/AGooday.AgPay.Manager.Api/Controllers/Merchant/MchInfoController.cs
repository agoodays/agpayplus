using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户管理类
    /// </summary>
    [Route("/api/mchInfo")]
    [ApiController, Authorize]
    public class MchInfoController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<MchInfoController> _logger;
        private readonly IMchInfoService _mchInfoService;
        private readonly ISysUserService _sysUserService;

        private readonly DomainNotificationHandler _notifications;

        public MchInfoController(IMQSender mqSender, ILogger<MchInfoController> logger, INotificationHandler<DomainNotification> notifications,
            IMchInfoService mchInfoService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _mchInfoService = mchInfoService;
            _sysUserService = sysUserService;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 商户信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_LIST)]
        public ApiPageRes<MchInfoDto> List([FromQuery] MchInfoQueryDto dto)
        {
            var data = _mchInfoService.GetPaginatedData(dto);
            return ApiPageRes<MchInfoDto>.Pages(data);
        }

        /// <summary>
        /// 新增商户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增商户信息")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_INFO_ADD)]
        public ApiRes Add(MchInfoCreateDto dto)
        {
            var sysUser = GetCurrentUser().SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            _mchInfoService.CreateAsync(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 删除商户信息
        /// </summary>
        /// <param name="mchNo"></param>
        /// <returns></returns>
        [HttpDelete, Route("{mchNo}"), MethodLog("删除商户信息")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_INFO_DEL)]
        public async Task<ApiRes> DeleteAsync(string mchNo)
        {
            await _mchInfoService.RemoveAsync(mchNo);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{mchNo}"), MethodLog("更新商户信息")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_INFO_EDIT)]
        public async Task<ApiRes> UpdateAsync(string mchNo, MchInfoModifyDto dto)
        {
            await _mchInfoService.ModifyAsync(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 查询商户信息
        /// </summary>
        /// <param name="mchNo"></param>
        /// <returns></returns>
        [HttpGet, Route("{mchNo}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_INFO_VIEW, PermCode.MGR.ENT_MCH_INFO_EDIT)]
        public ApiRes Detail(string mchNo)
        {
            var mchInfo = _mchInfoService.GetById(mchNo);
            if (mchInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var sysUser = _sysUserService.GetById(mchInfo.InitUserId.Value);
            if (sysUser != null)
            {
                mchInfo.AddExt("loginUsername", sysUser.LoginUsername);
            }
            return ApiRes.Ok(mchInfo);
        }
    }
}
