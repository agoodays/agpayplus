using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Order
{
    /// <summary>
    /// 退款订单类
    /// </summary>
    [Route("/api/refundOrder")]
    [ApiController]
    public class RefundOrderController : CommonController
    {
        private readonly ILogger<RefundOrderController> _logger;
        private readonly IRefundOrderService _refundOrderService;

        public RefundOrderController(ILogger<RefundOrderController> logger,
            IRefundOrderService refundOrderService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
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
        public ApiRes List([FromQuery] RefundOrderQueryDto dto)
        {
            dto.MchNo = GetCurrentUser().User.BelongInfoId;
            var refundOrders = _refundOrderService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = refundOrders.ToList(), Total = refundOrders.TotalCount, Current = refundOrders.PageIndex, HasNext = refundOrders.HasNext });
        }

        /// <summary>
        /// 退款订单信息
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        [HttpGet, Route("{refundOrderId}")]
        public ApiRes Detail(string refundOrderId)
        {
            var refundOrder = _refundOrderService.GetById(refundOrderId);
            if (refundOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (refundOrder.MchNo.Equals(GetCurrentUser().User.BelongInfoId))
            {
                return ApiRes.Fail(ApiCode.SYS_PERMISSION_ERROR);
            }
            return ApiRes.Ok(refundOrder);
        }
    }
}
