using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [ApiController]
    [Route("api/sysUsers")]
    public class SysUserController : ControllerBase
    {
        private readonly ILogger<SysUserController> _logger;
        private readonly ISysUserService _sysUserService;
        private IMemoryCache _cache;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        public SysUserController(ILogger<SysUserController> logger, ISysUserService sysUserService, IMemoryCache cache, INotificationHandler<DomainNotification> notifications)
        {
            _logger = logger;
            _sysUserService = sysUserService;
            _cache = cache;
            _notifications = (DomainNotificationHandler)notifications;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List([FromBody] SysUserVM vm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            vm.SysType = CS.SYS_TYPE.MGR;
            var data = _sysUserService.GetPaginatedData(vm, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public ApiRes Add(SysUserVM vm)
        {
            //_cache.Remove("ErrorData");
            vm.IsAdmin = CS.NO;
            vm.SysType = CS.SYS_TYPE.MGR;
            vm.BelongInfoId = "0";
            _sysUserService.Create(vm);
            //var errorData = _cache.Get("ErrorData");
            //if (errorData == null)
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        [HttpDelete]
        [Route("delete/{recordId}")]
        public ApiRes Delete(long recordId)
        {
            var currentUserId = 0;
            _sysUserService.Remove(recordId, currentUserId, CS.SYS_TYPE.MGR);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        [HttpPut]
        [Route("update/{recordId}")]
        public ApiRes Update(ModifySysUserVM vm)
        {
            vm.SysType = CS.SYS_TYPE.MGR;
            _sysUserService.Modify(vm);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        [HttpGet]
        [Route("detail/{recordId}")]
        public ApiRes Detail(long recordId)
        {
            var sysUser = _sysUserService.GetById(recordId);
            if (sysUser == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysUser);
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<int> Get()
        {
            return Enumerable.Range(1, 5).Select(index => Random.Shared.Next(index, 55)).ToArray();
        }
    }
}