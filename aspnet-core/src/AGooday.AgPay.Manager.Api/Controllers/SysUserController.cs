using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        public ApiRes List()
        {
            var users = _sysUserService.GetAll();
            return ApiRes.Ok(users);
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
            vm.SysType = CS.SYS_TYPE.MGR;
            _sysUserService.Create(vm);
            //var errorData = _cache.Get("ErrorData");
            //if (errorData == null)
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(string.Join(";", _notifications.GetNotifications().Select(s => s.Value)));
        }

        [HttpDelete]
        [Route("delete/{recordId}")]
        public ApiRes Delete(long recordId)
        {
            _sysUserService.Remove(recordId);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{recordId}")]
        public ApiRes Update(SysUserVM vm)
        {
            _sysUserService.Update(vm);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{recordId}")]
        public ApiRes Detail(long recordId)
        {
            return ApiRes.Ok(_sysUserService.GetById(recordId));
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<int> Get()
        {
            return Enumerable.Range(1, 5).Select(index => Random.Shared.Next(index, 55)).ToArray();
        }
    }
}