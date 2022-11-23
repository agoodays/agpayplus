using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayTest
{
    [Route("api/anon/paytestNotify")]
    [ApiController, AllowAnonymous, NoLog]
    public class PayTestNotifyController : ControllerBase
    {
        private readonly ILogger<PayTestNotifyController> _logger;
        private readonly IMchAppService _mchAppService;

        public PayTestNotifyController(ILogger<PayTestNotifyController> logger, IMchAppService mchAppService)
        {
            _logger = logger;
            _mchAppService = mchAppService;
        }

        [HttpPost, Route("payOrder")]
        public void PayOrderNotify(PayOrderNotifyModel payOrderNotify)
        {
           var mchApp = _mchAppService.GetById(payOrderNotify.AppId); 
            if (mchApp == null || !mchApp.MchNo.Equals(payOrderNotify.MchNo))
            {
                Response.WriteAsync("app is not exists");
                return;
            }
        }
    }
}
