using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Exceptions;

namespace AGooday.AgPay.Manager.Api.Controllers.PayConfig
{
    [Route("/api/payWay")]
    [ApiController]
    public class PayWayController : ControllerBase
    {
        private readonly ILogger<PayWayController> _logger;
        private readonly IPayWayService _payWayService;

        public PayWayController(ILogger<PayWayController> logger, IPayWayService payWayService)
        {
            _logger = logger;
            _payWayService = payWayService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List([FromBody] PayWayDto dto, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _payWayService.GetPaginatedData(dto, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(PayWayDto dto)
        {
            if (_payWayService.IsExistPayWayCode(dto.WayCode))
            {
                throw new BizException("支付方式代码已存在");
            }
            _payWayService.Add(dto);
            return ApiRes.Ok();
        }

        [HttpDelete]
        [Route("delete/{wayCode}")]
        public ApiRes Delete(string wayCode)
        {
            _payWayService.Remove(wayCode);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{wayCode}")]
        public ApiRes Update(PayWayDto dto)
        {
            _payWayService.Update(dto);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{wayCode}")]
        public ApiRes Detail(string wayCode)
        {
            var payWay = _payWayService.GetById(wayCode);
            if (payWay == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(payWay);
        }
    }
}
