using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AGooday.AgPay.Manager.Api.Controllers.Order
{
    /// <summary>
    /// 退款订单类
    /// </summary>
    [Route("api/refundOrder")]
    [ApiController, Authorize, NoLog]
    public class RefundOrderController : CommonController
    {
        private readonly IRefundOrderService _refundOrderService;
        private readonly IPayInterfaceDefineService _payIfDefineService;

        public RefundOrderController(ILogger<RefundOrderController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IRefundOrderService refundOrderService,
            IPayInterfaceDefineService payIfDefineService)
            : base(logger, cacheService, authService)
        {
            _refundOrderService = refundOrderService;
            _payIfDefineService = payIfDefineService;
        }

        /// <summary>
        /// 退款订单信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MGR.ENT_REFUND_LIST)]
        public async Task<ApiPageRes<RefundOrderDto>> ListAsync([FromQuery] RefundOrderQueryDto dto)
        {
            dto.BindDateRange();
            var refundOrders = await _refundOrderService.GetPaginatedDataAsync(dto);
            var ifDefines = _payIfDefineService.GetAllAsNoTracking();

            foreach (var refundOrder in refundOrders)
            {
                // 存入支付方式名称
                var ifDefine = ifDefines.FirstOrDefault(f => f.IfCode.Equals(refundOrder.IfCode));
                if (ifDefine != null)
                {
                    refundOrder.AddExt("ifName", ifDefine.IfName);
                    refundOrder.AddExt("bgColor", ifDefine.BgColor);
                    refundOrder.AddExt("icon", ifDefine.Icon);
                }
            }
            return ApiPageRes<RefundOrderDto>.Pages(refundOrders);
        }

        /// <summary>
        /// 退款订单信息统计
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("count"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_REFUND_LIST)]
        public async Task<ApiRes> CountAsync([FromQuery] RefundOrderQueryDto dto)
        {
            dto.BindDateRange();
            var statistics = await _refundOrderService.StatisticsAsync(dto);
            return ApiRes.Ok(statistics);
        }

        /// <summary>
        /// 订单信息导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("export/{bizType}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_REFUND_LIST)]
        public async Task<IActionResult> ExportAsync(string bizType, [FromQuery] RefundOrderQueryDto dto)
        {
            if (!"excel".Equals(bizType))
            {
                throw new BizException($"暂不支持{bizType}导出");
            }
            dto.BindDateRange();
            // 从数据库中检索需要导出的数据
            var refundOrders = await _refundOrderService.GetPaginatedDataAsync(dto);

            string fileName = $"退款订单.xlsx";
            // 5.0之后的epplus需要指定 商业证书 或者非商业证书。低版本不需要此行代码
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<dynamic> excelHeaders = new List<dynamic>() {
                new { Key = "refundOrderId", Width = 30d, Value = "退款订单号", },
                new { Key = "payOrderId", Width = 30d, Value = "支付订单号" },
                new { Key = "channelPayOrderNo", Width = 35d, Value = "渠道支付单号" },
                new { Key = "mchNo", Width = 25d, Value = "商户号" },
                new { Key = "isvNo", Width = 25d, Value = "服务商号" },
                new { Key = "appId", Width = 32d, Value = "应用ID" },
                new { Key = "mchName", Width = 25d, Value = "商户名称" },
                new { Key = "mchRefundNo", Width = 30d, Value = "商户退款单号" },
                new { Key = "wayCode", Width = 12d, Value = "支付方式代码" },
                new { Key = "ifCode", Width = 12d, Value = "支付接口代码" },
                new { Key = "payAmount", Width = 10d, Value = "支付金额" },
                new { Key = "refundAmount", Width = 10d, Value = "退款金额" },
                new { Key = "state", Width = 10d, Value = "退款状态" },
                new { Key = "refundReason", Width = 30d, Value = "退款原因" },
                new { Key = "successTime", Width = 23d, Value = "订单退款成功时间" },
                new { Key = "createdAt", Width = 23d, Value = "创建时间" }
            };
            // 创建新的 Excel 文件
            using (var package = new ExcelPackage())
            {
                // 添加工作表，并设置标题行
                var worksheet = package.Workbook.Worksheets.Add("退款订单");
                worksheet.Cells[1, 1].Value = $"退款订单";

                for (int i = 0; i < excelHeaders.Count; i++)
                {
                    var excelHeader = excelHeaders[i];
                    worksheet.Cells[2, i + 1].Value = excelHeader.Value;
                    worksheet.Column(i + 1).Width = excelHeader.Width;
                }
                // 固定前两行，第一列，`FreezePanes()`方法的第一个参数设置为3，表示从第三行开始向下滚动时会被冻结，第二个参数设置为3，表示从第二行开始向右滚动时会被冻结
                worksheet.View.FreezePanes(3, 2);
                // 将每个订单添加到工作表中
                for (int i = 0; i < refundOrders.Count; i++)
                {
                    var refundOrder = refundOrders[i];
                    var refundOrderJO = JObject.FromObject(refundOrder);
                    for (int j = 0; j < excelHeaders.Count; j++)
                    {
                        var excelHeader = excelHeaders[j];
                        var value = refundOrderJO[excelHeader.Key];
                        value = excelHeader.Key switch
                        {
                            "state" => refundOrder.State.ToEnum<RefundOrderState>()?.GetDescription() ?? "未知",
                            "payAmount" or "refundAmount" => Convert.ToDecimal(value) / 100,
                            _ => Convert.ToString(value),
                        };
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
                var rows = refundOrders.Count + 3;
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
        /// 退款订单信息
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        [HttpGet, Route("{refundOrderId}")]
        [PermissionAuth(PermCode.MGR.ENT_REFUND_ORDER_VIEW)]
        public async Task<ApiRes> DetailAsync(string refundOrderId)
        {
            var refundOrder = await _refundOrderService.GetByIdAsync(refundOrderId);
            if (refundOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(refundOrder);
        }
    }
}
