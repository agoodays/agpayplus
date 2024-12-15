using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayConfig
{
    [Route("api/payIfDefines")]
    [ApiController, Authorize]
    public class PayInterfaceDefineController : CommonController
    {
        private readonly IPayInterfaceDefineService _payIfDefineService;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public PayInterfaceDefineController(ILogger<PayInterfaceDefineController> logger,
            IPayInterfaceDefineService payIfDefineService,
            IPayInterfaceConfigService payIfConfigService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _payIfDefineService = payIfDefineService;
            _payIfConfigService = payIfConfigService;
        }

        /// <summary>
        /// 支付接口
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_MCH_PAY_CONFIG_LIST)]
        public async Task<ApiRes> ListAsync()
        {
            var data = await _payIfConfigService.GetPayIfConfigsByMchNoAsync(GetCurrentMchNo());
            return ApiRes.Ok(data);
        }
    }
}
