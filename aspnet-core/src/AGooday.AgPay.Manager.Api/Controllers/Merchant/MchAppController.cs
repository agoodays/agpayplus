using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
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
        public ApiRes List([FromBody] MchAppDto dto, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _mchAppService.GetPaginatedData(dto, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(MchAppDto dto)
        {
            _mchAppService.Add(dto);
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
        public ApiRes Update(MchAppDto dto)
        {
            _mchAppService.Update(dto);
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
