using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AGooday.AgPay.Merchant.Api.Controllers.Order
{
    /// <summary>
    /// 转账订单
    /// </summary>
    [Route("/api/transferOrders")]
    [ApiController, Authorize, NoLog]
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
        /// 订单信息导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("export/{bizType}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_TRANSFER_ORDER_LIST)]
        public IActionResult Export(string bizType, [FromQuery] TransferOrderQueryDto dto)
        {
            dto.BindDateRange();
            dto.BindDateRange();
            var transferOrders = _transferOrderService.GetPaginatedData(dto);
            string fileName = $"转账订单.xlsx";
            // 5.0之后的epplus需要指定 商业证书 或者非商业证书。低版本不需要此行代码
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<dynamic> excelHeaders = new List<dynamic>() {
                new { Key = "transferId", Width = 30d, Value = "转账订单号", },
                new { Key = "mchNo", Width = 30d, Value = "商户号", },
                new { Key = "isvNo", Width = 35d, Value = "服务商号", },
                new { Key = "appId", Width = 25d, Value = "应用ID", },
                new { Key = "mchName", Width = 25d, Value = "商户名称", },
                new { Key = "mchOrderNo", Width = 32d, Value = "商户订单号", },
                new { Key = "ifCode", Width = 12d, Value = "支付接口代码", },
                new { Key = "state", Width = 10d, Value = "转账状态", },
                new { Key = "amount", Width = 10d, Value = "转账金额", },
                new { Key = "accountNo", Width = 25d, Value = "收款账号", },
                new { Key = "accountName", Width = 20d, Value = "收款人姓名", },
                new { Key = "bankName", Width = 30d, Value = "收款人开户行名称", },
                new { Key = "transferDesc", Width = 30d, Value = "转账备注信息", },
                new { Key = "successTime", Width = 23d, Value = "转账成功时间", },
                new { Key = "createdAt", Width = 23d, Value = "创建时间" },
            };
            // 创建新的 Excel 文件
            using (var package = new ExcelPackage())
            {
                // 添加工作表，并设置标题行
                var worksheet = package.Workbook.Worksheets.Add(Name: "转账订单");
                worksheet.Cells[1, 1].Value = $"转账订单";

                for (int i = 0; i < excelHeaders.Count; i++)
                {
                    var excelHeader = excelHeaders[i];
                    worksheet.Cells[2, i + 1].Value = excelHeader.Value;
                    worksheet.Column(i + 1).Width = excelHeader.Width;
                }
                // 固定前两行，第一列，`FreezePanes()`方法的第一个参数设置为3，表示从第三行开始向下滚动时会被冻结，第二个参数设置为3，表示从第二行开始向右滚动时会被冻结
                worksheet.View.FreezePanes(3, 2);
                // 将每个订单添加到工作表中
                for (int i = 0; i < transferOrders.Count(); i++)
                {
                    var transferOrder = transferOrders[i];
                    var refundOrderJO = JObject.FromObject(transferOrder);
                    for (int j = 0; j < excelHeaders.Count; j++)
                    {
                        var excelHeader = excelHeaders[j];
                        var value = refundOrderJO[excelHeader.Key];
                        switch (excelHeader.Key)
                        {
                            case "state":
                                value = transferOrder.State.ToEnum<TransferOrderState>()?.GetDescription() ?? "未知";
                                break;
                            case "amount":
                                value = Convert.ToDecimal(value) / 100;
                                break;
                            default:
                                value = Convert.ToString(value);
                                break;
                        }
                        worksheet.Cells[i + 3, j + 1].Value = value;
                    }
                }
                //// 全局样式
                //worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;// 水平居中
                //worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;// 垂直居中
                //worksheet.Cells.AutoFitColumns();
                //worksheet.Cells.Style.WrapText = true;// 自动换行
                //worksheet.Cells.Style.Font.Name = "宋体";
                //worksheet.Rows.Height = 25;

                // 设置单元格样式，例如居中对齐和加粗字体
                var cols = excelHeaders.Count + 1;
                var rows = transferOrders.Count() + 3;
                for (int i = 1; i < rows; i++)
                {
                    worksheet.Row(i).Height = 25;
                }
                worksheet.Cells[1, 1, 1, cols].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, cols].Merge = true;
                worksheet.Cells[1, 1, rows, cols].Style.WrapText = true;// 自动换行
                worksheet.Cells[1, 1, rows, cols].Style.Font.Name = "等线";
                worksheet.Cells[1, 1, rows, cols].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1, rows, cols].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
