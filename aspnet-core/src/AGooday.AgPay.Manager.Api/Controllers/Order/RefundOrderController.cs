using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Order
{
    /// <summary>
    /// 退款订单类
    /// </summary>
    [Route("/api/refundOrder")]
    [ApiController, Authorize, NoLog]
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

        /// <summary>
        /// 退款订单信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MGR.ENT_REFUND_LIST)]
        public ApiRes List([FromQuery] RefundOrderQueryDto dto)
        {
            var refundOrders = _refundOrderService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = refundOrders.ToList(), Total = refundOrders.TotalCount, Current = refundOrders.PageIndex, HasNext = refundOrders.HasNext });
        }

        /// <summary>
        /// 退款订单信息
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        [HttpGet, Route("{refundOrderId}")]
        [PermissionAuth(PermCode.MGR.ENT_REFUND_ORDER_VIEW)]
        public ApiRes Detail(string refundOrderId)
        {
            var refundOrder = _refundOrderService.GetById(refundOrderId);
            if (refundOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(refundOrder);
        }
    }
}
