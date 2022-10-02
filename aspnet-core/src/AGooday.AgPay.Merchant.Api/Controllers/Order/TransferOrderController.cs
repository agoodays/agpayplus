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
    /// 转账订单
    /// </summary>
    [Route("/api/transferOrders")]
    [ApiController]
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
        public ApiRes List([FromQuery] TransferOrderQueryDto dto)
        {
            dto.MchNo = GetCurrentUser().User.BelongInfoId;
            var transferOrders = _transferOrderService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = transferOrders.ToList(), Total = transferOrders.TotalCount, Current = transferOrders.PageIndex, HasNext = transferOrders.HasNext });
        }

        /// <summary>
        /// 转账订单信息
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        [HttpGet, Route("{transferId}")]
        public ApiRes Detail(string transferId)
        {
            var refundOrder = _transferOrderService.QueryMchOrder(GetCurrentUser().User.BelongInfoId, null, transferId);
            if (refundOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(refundOrder);
        }
    }
}
