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
    [Route("/api/payIfDefine")]
    [ApiController]
    public class PayInterfaceDefineController : ControllerBase
    {
        private readonly ILogger<PayInterfaceDefineController> _logger;
        private readonly IPayInterfaceDefineService _payIfDefineService;
        private readonly IPayInterfaceConfigService _payIfConfigService;
        private readonly IPayOrderService _payOrderService;

        public PayInterfaceDefineController(ILogger<PayInterfaceDefineController> logger,
            IPayInterfaceDefineService payIfDefineService,
            IPayInterfaceConfigService payIfConfigService,
            IPayOrderService payOrderService)
        {
            _logger = logger;
            _payIfDefineService = payIfDefineService;
            _payIfConfigService = payIfConfigService;
            _payOrderService = payOrderService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List()
        {
            var data = _payIfDefineService.GetAll();
            return ApiRes.Ok(data);
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(PayInterfaceDefineDto dto)
        {
            _payIfDefineService.Add(dto);
            return ApiRes.Ok();
        }

        [HttpDelete]
        [Route("delete/{ifCode}")]
        public ApiRes Delete(string ifCode)
        {
            // 校验该支付方式是否有服务商或商户配置参数或者已有订单
            if (_payIfConfigService.IsExistUseIfCode(ifCode) || _payOrderService.IsExistOrderUseIfCode(ifCode))
            {
                throw new BizException("该支付接口已有服务商或商户配置参数或已发生交易，无法删除！");
            }
            _payIfDefineService.Remove(ifCode);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{ifCode}")]
        public ApiRes Update(PayInterfaceDefineDto dto)
        {
            _payIfDefineService.Update(dto);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{ifCode}")]
        public ApiRes Detail(string ifCode)
        {
            var payIfDefine = _payIfDefineService.GetById(ifCode);
            if (payIfDefine == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(payIfDefine);
        }
    }
}
