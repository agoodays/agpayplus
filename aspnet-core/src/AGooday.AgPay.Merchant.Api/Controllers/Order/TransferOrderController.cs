using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Order
{
    /// <summary>
    /// 转账订单
    /// </summary>
    [Route("/api/transferOrders")]
    [ApiController, Authorize]
    public class TransferOrderController : CommonController
    {
        private readonly ILogger<TransferOrderController> _logger;
        private readonly ITransferOrderService _transferOrderService;

        public TransferOrderController(ILogger<TransferOrderController> logger,
            ITransferOrderService transferOrderService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _transferOrderService = transferOrderService;
        }

        /// <summary>
        /// 转账订单信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MCH.ENT_TRANSFER_ORDER_LIST)]
        public ApiRes List([FromQuery] TransferOrderQueryDto dto)
        {
            dto.MchNo = GetCurrentMchNo();
            var transferOrders = _transferOrderService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = transferOrders.ToList(), Total = transferOrders.TotalCount, Current = transferOrders.PageIndex, HasNext = transferOrders.HasNext });
        }

        /// <summary>
        /// 转账订单信息
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        [HttpGet, Route("{transferId}")]
        [PermissionAuth(PermCode.MCH.ENT_TRANSFER_ORDER_VIEW)]
        public ApiRes Detail(string transferId)
        {
            var refundOrder = _transferOrderService.QueryMchOrder(GetCurrentMchNo(), null, transferId);
            if (refundOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(refundOrder);
        }
    }
}
