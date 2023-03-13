using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Order
{
    /// <summary>
    /// 转账订单
    /// </summary>
    [Route("/api/transferOrders")]
    [ApiController, Authorize, NoLog]
    public class TransferOrderController : ControllerBase
    {
        private readonly ILogger<TransferOrderController> _logger;
        private readonly ITransferOrderService _transferOrderService;

        public TransferOrderController(ILogger<TransferOrderController> logger,
            ITransferOrderService transferOrderService)
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
        [PermissionAuth(PermCode.MGR.ENT_TRANSFER_ORDER_LIST)]
        public ApiRes List([FromQuery] TransferOrderQueryDto dto)
        {
            dto.BindDateRange();
            var transferOrders = _transferOrderService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = transferOrders.ToList(), Total = transferOrders.TotalCount, Current = transferOrders.PageIndex, HasNext = transferOrders.HasNext });
        }

        /// <summary>
        /// 订单信息导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("export/{bizType}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_REFUND_LIST)]
        public IActionResult Export([FromQuery] TransferOrderQueryDto dto, string bizType)
        {
            dto.BindDateRange();
            var transferOrders = _transferOrderService.GetPaginatedData(dto);
            string fileName = $"转账订单.xlsx";
            //store in memory rather than pysical directory
            var stream = new MemoryStream();
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// 转账订单信息
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        [HttpGet, Route("{transferId}")]
        [PermissionAuth(PermCode.MGR.ENT_TRANSFER_ORDER_VIEW)]
        public ApiRes Detail(string transferId)
        {
            var refundOrder = _transferOrderService.GetById(transferId);
            if (refundOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(refundOrder);
        }
    }
}
