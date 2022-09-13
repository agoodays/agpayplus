using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [ApiController]
    [Route("api/sysUsers")]
    public class SysUserController : ControllerBase
    {
        private readonly ILogger<SysUserController> _logger;
        private readonly ISysUserService _sysUserService;

        public SysUserController(ILogger<SysUserController> logger, ISysUserService sysUserService)
        {
            _logger = logger;
            _sysUserService = sysUserService;
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
            vm.SysType = CS.SYS_TYPE.MGR;
            _sysUserService.Create(vm);
            return ApiRes.Ok();
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