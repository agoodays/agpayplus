using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 服务商支付接口管理类
    /// </summary>
    [Route("/api/mch/payConfigs")]
    [ApiController]
    public class MchPayInterfaceConfigController : ControllerBase
    {
        private readonly ILogger<MchPayInterfaceConfigController> _logger;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public MchPayInterfaceConfigController(ILogger<MchPayInterfaceConfigController> logger,
            IPayInterfaceConfigService payIfConfigService,
            IPayOrderService payOrderService)
        {
            _logger = logger;
            _payIfConfigService = payIfConfigService;
        }

        /// <summary>
        /// 查询应用支付接口配置列表
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ApiRes List(string isvNo)
        {
            var data = _payIfConfigService.SelectAllPayIfConfigListByIsvNo(CS.INFO_TYPE_ISV, isvNo);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 根据 appId、接口类型 获取应用参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{isvNo}/{ifCode}")]
        public ApiRes GetByAppId(string appId, string ifCode)
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 应用支付接口配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ApiRes SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("alipayIsvsubMchAuthUrls/{mchAppId}")]
        public ApiRes QueryAlipayIsvsubMchAuthUrl(string mchAppId)
        {
            return ApiRes.Ok(new { authUrl = "", authQrImgUrl = "" });
        }
    }
}
