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
    /// <summary>
    /// 服务商支付接口管理类
    /// </summary>
    [Route("/api/isv/payConfigs")]
    [ApiController]
    public class IsvPayInterfaceConfigController : ControllerBase
    {
        private readonly ILogger<IsvPayInterfaceConfigController> _logger;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public IsvPayInterfaceConfigController(ILogger<IsvPayInterfaceConfigController> logger,
            IPayInterfaceConfigService payIfConfigService,
            IPayOrderService payOrderService)
        {
            _logger = logger;
            _payIfConfigService = payIfConfigService;
        }

        [HttpGet]
        [Route("")]
        public ApiRes List(string isvNo)
        {
            var data = _payIfConfigService.SelectAllPayIfConfigListByIsvNo(CS.INFO_TYPE_ISV, isvNo);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 根据 服务商号、接口类型 获取商户参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{isvNo}/{ifCode}")]
        public ApiRes GetByIsvNo(string isvNo, string ifCode)
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 服务商支付接口参数配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ApiRes SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            return ApiRes.Ok();
        }
    }
}
