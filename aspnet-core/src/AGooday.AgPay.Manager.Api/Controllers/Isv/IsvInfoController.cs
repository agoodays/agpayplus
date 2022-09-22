using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Isv
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
        public ApiRes List([FromBody] IsvInfoDto dto, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _isvInfoService.GetPaginatedData(dto, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(IsvInfoDto dto)
        {
            _isvInfoService.Add(dto);
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
        public ApiRes Update(IsvInfoDto dto)
        {
            _isvInfoService.Update(dto);
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
