using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("/api/mchApp")]
    [ApiController]
    public class MchAppController : ControllerBase
    {
        private readonly ILogger<MchAppController> _logger;
        private readonly IMchAppService _mchAppService;

        public MchAppController(ILogger<MchAppController> logger, IMchAppService mchAppService)
        {
            _logger = logger;
            _mchAppService = mchAppService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List()
        {
            var data = _mchAppService.GetAll();
            return ApiRes.Ok(data);
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(MchAppVM vm)
        {
            _mchAppService.Add(vm);
            return ApiRes.Ok();
        }

        [HttpDelete]
        [Route("delete/{appId}")]
        public ApiRes Delete(string appId)
        {
            _mchAppService.Remove(appId);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{appId}")]
        public ApiRes Update(MchAppVM vm)
        {
            _mchAppService.Update(vm);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{appId}")]
        public ApiRes Detail(string appId)
        {
            var mchApp = _mchAppService.GetById(appId);
            if (mchApp == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchApp);
        }
    }
}
