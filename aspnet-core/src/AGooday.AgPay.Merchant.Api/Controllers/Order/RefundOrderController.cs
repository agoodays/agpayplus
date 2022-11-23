using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Order
{
    /// <summary>
    /// 退款订单类
    /// </summary>
    [Route("/api/refundOrder")]
    [ApiController, Authorize, NoLog]
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
        [PermissionAuth(PermCode.MCH.ENT_REFUND_LIST)]
        public ApiRes List([FromQuery] RefundOrderQueryDto dto)
        {
            dto.MchNo = GetCurrentMchNo();
            var refundOrders = _refundOrderService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = refundOrders.ToList(), Total = refundOrders.TotalCount, Current = refundOrders.PageIndex, HasNext = refundOrders.HasNext });
        }

        /// <summary>
        /// 退款订单信息
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        [HttpGet, Route("{refundOrderId}")]
        [PermissionAuth(PermCode.MCH.ENT_REFUND_ORDER_VIEW)]
        public ApiRes Detail(string refundOrderId)
        {
            var refundOrder = _refundOrderService.GetById(refundOrderId);
            if (refundOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (!refundOrder.MchNo.Equals(GetCurrentMchNo()))
            {
                return ApiRes.Fail(ApiCode.SYS_PERMISSION_ERROR);
            }
            return ApiRes.Ok(refundOrder);
        }
    }
}
