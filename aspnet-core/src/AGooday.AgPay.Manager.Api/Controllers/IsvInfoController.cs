using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("/api/isvInfo")]
    [ApiController]
    public class IsvInfoController : ControllerBase
    {
        private readonly ILogger<IsvInfoController> _logger;
        private readonly IIsvInfoService _isvInfoService;

        public IsvInfoController(ILogger<IsvInfoController> logger, IIsvInfoService isvInfoService)
        {
            _logger = logger;
            _isvInfoService = isvInfoService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List([FromBody] IsvInfoVM vm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _isvInfoService.GetPaginatedData(vm, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(IsvInfoVM vm)
        {
            _isvInfoService.Add(vm);
            return ApiRes.Ok();
        }

        [HttpDelete]
        [Route("delete/{isvNo}")]
        public ApiRes Delete(string isvNo)
        {
            _isvInfoService.Remove(isvNo);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{isvNo}")]
        public ApiRes Update(IsvInfoVM vm)
        {
            _isvInfoService.Update(vm);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{isvNo}")]
        public ApiRes Detail(string isvNo)
        {
            var isvInfo = _isvInfoService.GetById(isvNo);
            if (isvInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(isvInfo);
        }
    }
}
