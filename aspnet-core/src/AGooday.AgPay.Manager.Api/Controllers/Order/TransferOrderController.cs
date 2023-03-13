using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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
        public IActionResult Export(string bizType, [FromQuery] TransferOrderQueryDto dto)
        {
            dto.BindDateRange();
            var transferOrders = _transferOrderService.GetPaginatedData(dto);
            string fileName = $"转账订单.xlsx";
            // 5.0之后的epplus需要指定 商业证书 或者非商业证书。低版本不需要此行代码
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // 创建新的 Excel 文件
            using (var package = new ExcelPackage())
            {
                // 添加工作表，并设置标题行
                var worksheet = package.Workbook.Worksheets.Add("转账订单");
                worksheet.Cells["A1"].Value = $"转账订单";
                worksheet.Column(1).Width = 30d;
                worksheet.Cells["A2"].Value = $"转账订单号";
                worksheet.Column(2).Width = 25d;
                worksheet.Cells["B2"].Value = $"商户号";
                worksheet.Column(3).Width = 25d;
                worksheet.Cells["C2"].Value = $"商户名称";
                worksheet.Column(4).Width = 10d;
                worksheet.Cells["D2"].Value = $"支付状态";
                worksheet.Column(5).Width = 10d;
                worksheet.Cells["E2"].Value = $"转账金额";
                worksheet.Column(14).Width = 23d;
                worksheet.Cells["F2"].Value = $"创建时间";
                worksheet.Column(15).Width = 23d;
                worksheet.Cells["G2"].Value = $"支付成功时间";
                // 将每个订单添加到工作表中
                for (int i = 0; i < transferOrders.Count(); i++)
                {
                    var order = transferOrders[i];
                    worksheet.Cells[$"A{i + 3}"].Value = order.TransferId;
                    worksheet.Cells[$"B{i + 3}"].Value = order.MchNo;
                    worksheet.Cells[$"C{i + 3}"].Value = order.MchName;
                    worksheet.Cells[$"D{i + 3}"].Value = order.State;
                    worksheet.Cells[$"E{i + 3}"].Value = order.Amount / 100.00;
                    worksheet.Cells[$"F{i + 3}"].Value = order.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[$"G{i + 3}"].Value = order.SuccessTime?.ToString("yyyy-MM-dd HH:mm:ss");
                }
                //// 全局样式
                //worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;// 水平居中
                //worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;// 垂直居中
                //worksheet.Cells.AutoFitColumns();
                //worksheet.Cells.Style.WrapText = true;// 自动换行
                //worksheet.Cells.Style.Font.Name = "宋体";
                //worksheet.Rows.Height = 25;

                // 设置单元格样式，例如居中对齐和加粗字体
                var rows = transferOrders.Count() + 3;
                for (int i = 1; i < rows; i++)
                {
                    worksheet.Row(i).Height = 25;
                }
                worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                worksheet.Cells["A1:G1"].Merge = true;
                worksheet.Cells[$"A1:G{rows}"].Style.WrapText = true;// 自动换行
                worksheet.Cells[$"A1:G{rows}"].Style.Font.Name = "等线";
                worksheet.Cells[$"A1:G{rows}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"A1:G{rows}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //// 设置响应头，指示将要下载的文件类型为 Excel 文件
                //Response.Headers.Add("Content-Disposition", $"attachment;filename=\"{WebUtility.UrlEncode(fileName)}\"");
                //Response.ContentType = "application/vnd.ms-excel;charset=UTF-8";
                // 将 Excel 文件写入 HTTP 响应流中，并返回给客户端
                //return File(package.GetAsByteArray(), Response.ContentType, fileName);
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
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
