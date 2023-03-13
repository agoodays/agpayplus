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
            dto.BindDateRange();
            var refundOrders = _refundOrderService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = refundOrders.ToList(), Total = refundOrders.TotalCount, Current = refundOrders.PageIndex, HasNext = refundOrders.HasNext });
        }

        /// <summary>
        /// 订单信息导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("export/{bizType}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_REFUND_LIST)]
        public IActionResult Export(string bizType, [FromQuery] RefundOrderQueryDto dto)
        {
            dto.BindDateRange();
            // 从数据库中检索需要导出的数据
            var refundOrders = _refundOrderService.GetPaginatedData(dto);

            string fileName = $"退款订单.xlsx";
            // 5.0之后的epplus需要指定 商业证书 或者非商业证书。低版本不需要此行代码
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // 创建新的 Excel 文件
            using (var package = new ExcelPackage())
            {
                // 添加工作表，并设置标题行
                var worksheet = package.Workbook.Worksheets.Add("退款订单");
                worksheet.Cells["A1"].Value = $"退款订单";
                worksheet.Column(1).Width = 30d;
                worksheet.Cells["A2"].Value = $"退款订单号";
                worksheet.Column(1).Width = 30d;
                worksheet.Cells["B2"].Value = $"支付订单号";
                worksheet.Column(3).Width = 25d;
                worksheet.Cells["C2"].Value = $"商户号";
                worksheet.Column(4).Width = 25d;
                worksheet.Cells["D2"].Value = $"商户名称";
                worksheet.Column(5).Width = 15d;
                worksheet.Cells["E2"].Value = $"门店ID";
                worksheet.Column(6).Width = 22d;
                worksheet.Cells["F2"].Value = $"门店名称";
                worksheet.Column(7).Width = 10d;
                worksheet.Cells["G2"].Value = $"支付状态";
                worksheet.Column(8).Width = 10d;
                worksheet.Cells["H2"].Value = $"退款状态";
                worksheet.Column(9).Width = 25d;
                worksheet.Cells["I2"].Value = $"服务商号";
                worksheet.Column(10).Width = 12d;
                worksheet.Cells["J2"].Value = $"支付方式";
                worksheet.Column(11).Width = 10d;
                worksheet.Cells["K2"].Value = $"支付金额";
                worksheet.Column(12).Width = 10d;
                worksheet.Cells["L2"].Value = $"退款金额";
                worksheet.Column(13).Width = 10d;
                worksheet.Cells["M2"].Value = $"手续费";
                worksheet.Column(14).Width = 23d;
                worksheet.Cells["N2"].Value = $"创建时间";
                worksheet.Column(15).Width = 23d;
                worksheet.Cells["O2"].Value = $"支付成功时间";
                // 将每个订单添加到工作表中
                for (int i = 0; i < refundOrders.Count(); i++)
                {
                    var order = refundOrders[i];
                    worksheet.Cells[$"A{i + 3}"].Value = order.RefundOrderId;
                    worksheet.Cells[$"B{i + 3}"].Value = order.PayOrderId;
                    worksheet.Cells[$"C{i + 3}"].Value = order.MchNo;
                    worksheet.Cells[$"D{i + 3}"].Value = order.MchName;
                    worksheet.Cells[$"E{i + 3}"].Value = "";
                    worksheet.Cells[$"F{i + 3}"].Value = "";
                    worksheet.Cells[$"G{i + 3}"].Value = order.State;
                    worksheet.Cells[$"H{i + 3}"].Value = "";
                    worksheet.Cells[$"I{i + 3}"].Value = order.IsvNo;
                    worksheet.Cells[$"J{i + 3}"].Value = order.WayCode;
                    worksheet.Cells[$"K{i + 3}"].Value = order.PayAmount / 100.00;
                    worksheet.Cells[$"L{i + 3}"].Value = order.RefundAmount / 100.00;
                    worksheet.Cells[$"M{i + 3}"].Value = 0.00;
                    worksheet.Cells[$"N{i + 3}"].Value = order.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[$"O{i + 3}"].Value = order.SuccessTime?.ToString("yyyy-MM-dd HH:mm:ss");
                }
                //// 全局样式
                //worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;// 水平居中
                //worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;// 垂直居中
                //worksheet.Cells.AutoFitColumns();
                //worksheet.Cells.Style.WrapText = true;// 自动换行
                //worksheet.Cells.Style.Font.Name = "宋体";
                //worksheet.Rows.Height = 25;

                // 设置单元格样式，例如居中对齐和加粗字体
                var rows = refundOrders.Count() + 3;
                for (int i = 1; i < rows; i++)
                {
                    worksheet.Row(i).Height = 25;
                }
                worksheet.Cells["A1:O1"].Style.Font.Bold = true;
                worksheet.Cells["A1:O1"].Merge = true;
                worksheet.Cells[$"A1:O{rows}"].Style.WrapText = true;// 自动换行
                worksheet.Cells[$"A1:O{rows}"].Style.Font.Name = "等线";
                worksheet.Cells[$"A1:O{rows}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"A1:O{rows}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //// 设置响应头，指示将要下载的文件类型为 Excel 文件
                //Response.Headers.Add("Content-Disposition", $"attachment;filename=\"{WebUtility.UrlEncode(fileName)}\"");
                //Response.ContentType = "application/vnd.ms-excel;charset=UTF-8";
                // 将 Excel 文件写入 HTTP 响应流中，并返回给客户端
                //return File(package.GetAsByteArray(), Response.ContentType, fileName);
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
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
