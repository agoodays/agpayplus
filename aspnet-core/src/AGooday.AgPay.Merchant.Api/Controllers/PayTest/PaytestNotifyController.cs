using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Merchant.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayTest
{
    [Route("api/anon/paytestNotify")]
    [ApiController]
    public class PaytestNotifyController : ControllerBase
    {
        private readonly ILogger<PaytestNotifyController> _logger;

        public PaytestNotifyController(ILogger<PaytestNotifyController> logger)
        {
            _logger = logger;
        }

        [HttpPost, Route("payOrder")]
        public void PayOrderNotify(PayOrderNotifyModel payOrderNotify)
        {

        }
    }
}
