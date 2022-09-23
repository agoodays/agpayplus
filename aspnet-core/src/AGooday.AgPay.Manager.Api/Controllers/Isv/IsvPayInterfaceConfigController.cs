using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Exceptions;

namespace AGooday.AgPay.Manager.Api.Controllers.Isv
{
    [Route("/api/isv/payConfig")]
    [ApiController]
    public class IsvPayInterfaceConfigController : ControllerBase
    {
        private readonly ILogger<IsvPayInterfaceConfigController> _logger;
        private readonly IPayInterfaceConfigService _payIfConfigService;
        private readonly IPayOrderService _payOrderService;

        public IsvPayInterfaceConfigController(ILogger<IsvPayInterfaceConfigController> logger,
            IPayInterfaceConfigService payIfConfigService,
            IPayOrderService payOrderService)
        {
            _logger = logger;
            _payIfConfigService = payIfConfigService;
            _payOrderService = payOrderService;
        }

        [HttpGet]
        [Route("list/{isvNo}")]
        public ApiRes List(string isvNo)
        {
            var data = _payIfConfigService.SelectAllPayIfConfigListByIsvNo(CS.INFO_TYPE_ISV, isvNo);
            return ApiRes.Ok(data);
        }

        [HttpGet]
        [Route("{isvNo}/{ifCode}")]
        public ApiRes GetByIfCode(string isvNo, string ifCode)
        {
            return ApiRes.Ok();
        }

        [HttpPost]
        [Route("save")]
        public ApiRes Save(PayInterfaceConfigDto dto)
        {
            return ApiRes.Ok();
        }
    }
}
