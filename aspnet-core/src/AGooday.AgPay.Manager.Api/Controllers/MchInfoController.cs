using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("/api/mchInfo")]
    [ApiController]
    public class MchInfoController : ControllerBase
    {
        private readonly ILogger<MchInfoController> _logger;
        private readonly IMchInfoService _mchInfoService;

        public MchInfoController(ILogger<MchInfoController> logger, IMchInfoService mchInfoService)
        {
            _logger = logger;
            _mchInfoService = mchInfoService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List([FromBody] MchInfoDto dto, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _mchInfoService.GetPaginatedData(dto, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(MchInfoDto dto)
        {
            _mchInfoService.Add(dto);
            return ApiRes.Ok();
        }

        [HttpDelete]
        [Route("delete/{mchNo}")]
        public ApiRes Delete(string mchNo)
        {
            _mchInfoService.Remove(mchNo);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{mchNo}")]
        public ApiRes Update(MchInfoDto dto)
        {
            _mchInfoService.Update(dto);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{mchNo}")]
        public ApiRes Detail(string mchNo)
        {
            var mchInfo = _mchInfoService.GetById(mchNo);
            if (mchInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchInfo);
        }
    }
}
