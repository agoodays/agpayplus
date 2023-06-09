using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.QrCode
{
    [ApiController, Authorize]
    [Route("api/qrc")]
    public class QrCodeController : ControllerBase
    {
        private readonly ILogger<QrCodeController> _logger;

        public QrCodeController(ILogger<QrCodeController> logger)
        {
            _logger = logger;
        }
    }
}
