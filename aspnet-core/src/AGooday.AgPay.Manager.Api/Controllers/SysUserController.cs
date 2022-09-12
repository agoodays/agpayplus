using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("add")]
        public ApiRes Add(SysUserVM vm)
        {
            _sysUserService.Add(vm);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<int> Get()
        {
            return Enumerable.Range(1, 5).Select(index => Random.Shared.Next(index, 55)).ToArray();
        }
    }
}