using AGooday.AgPay.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Order
{
    [Route("/api/refundOrder")]
    [ApiController]
    public class RefundOrderController : ControllerBase
    {
        private readonly ILogger<RefundOrderController> _logger;
        private readonly IRefundOrderService _refundOrderService;

        public RefundOrderController(ILogger<RefundOrderController> logger, 
            IRefundOrderService refundOrderService)
        {
            _logger = logger;
            _refundOrderService = refundOrderService;
        }
    }
}
