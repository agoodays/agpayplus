using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AGooday.AgPay.Manager.Api.Controllers.Statistic
{
    /// <summary>
    /// 数据统计
    /// </summary>
    [Route("/api/statistic")]
    [ApiController, Authorize]
    public class StatisticController : CommonController
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly IStatisticService _statisticService;

        public StatisticController(ILogger<StatisticController> logger,
            IStatisticService statisticService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _statisticService = statisticService;
        }

        /// <summary>
        /// 统计列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_STATISTIC)]
        public ApiPageRes<StatisticResultDto> Statistics([FromQuery] StatisticQueryDto dto)
        {
            ChickAuth(dto.Method);
            dto.BindDateRange();
            var result = _statisticService.Statistics(dto);
            return ApiPageRes<StatisticResultDto>.Pages(result);
        }

        /// <summary>
        /// 统计合计
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("total"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_STATISTIC)]
        public ApiRes Total([FromQuery] StatisticQueryDto dto)
        {
            ChickAuth(dto.Method);
            dto.BindDateRange();
            var statistics = _statisticService.Total(dto);
            return ApiRes.Ok(statistics);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("export/{bizType}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_LIST)]
        public IActionResult Export(string bizType, [FromQuery] StatisticQueryDto dto)
        {
            ChickAuth(dto.Method);
            dto.BindDateRange();
            // 从数据库中检索需要导出的数据
            var result = _statisticService.Statistics(dto);

            string title = dto.Method switch
            {
                "transaction" => "交易报表",
                "mch" => "商户统计",
                _ => throw new NotImplementedException()
            };
            string fileName = $"{title}.xlsx";
            // 5.0之后的epplus需要指定 商业证书 或者非商业证书。低版本不需要此行代码
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<dynamic> excelHeaders = dto.Method switch
            {
                "transaction" => new List<dynamic>() { new { Key = "groupDate", Width = 20d, Value = $"日期" } },
                "mch" => new List<dynamic>() {
                    new { Key = "mchName", Width = 20d, Value = $"商户名称" },
                    new { Key = "mchNo", Width = 20d, Value = $"商户号" }
                },
                _ => throw new NotImplementedException()
            };
            excelHeaders.AddRange(new List<dynamic>() {
                new { Key = "payAmount", Width = 20d, Value = $"交易金额" },
                new { Key = "amount", Width = 20d, Value = $"实收金额" },
                new { Key = "refundAmount", Width = 20d, Value = $"退款金额" },
                new { Key = "refundCount", Width = 20d, Value = $"退款笔数" },
                new { Key = "payCount", Width = 20d, Value = $"支付成功笔数" },
                new { Key = "allCount", Width = 20d, Value = $"总交易笔数" },
                new { Key = "round", Width = 20d, Value = $"成功率" }
            });
            // 创建新的 Excel 文件
            using (var package = new ExcelPackage())
            {
                // 添加工作表，并设置标题行
                var worksheet = package.Workbook.Worksheets.Add(title);
                worksheet.Cells[1, 1].Value = title;

                for (int i = 0; i < excelHeaders.Count; i++)
                {
                    var excelHeader = excelHeaders[i];
                    worksheet.Cells[2, i + 1].Value = excelHeader.Value;
                    worksheet.Column(i + 1).Width = excelHeader.Width;
                }
                // 固定前两行，第一列，`FreezePanes()`方法的第一个参数设置为3，表示从第三行开始向下滚动时会被冻结，第二个参数设置为3，表示从第二行开始向右滚动时会被冻结
                worksheet.View.FreezePanes(3, 2);
                // 将每个订单添加到工作表中
                for (int i = 0; i < result.Count; i++)
                {
                    var item = result[i];
                    var orderJO = JObject.FromObject(item);
                    for (int j = 0; j < excelHeaders.Count; j++)
                    {
                        var excelHeader = excelHeaders[j];
                        var value = orderJO[excelHeader.Key];
                        value = excelHeader.Key switch
                        {
                            "payAmount" or "refundAmount" => (Convert.ToDecimal(value) / 100).ToString("0.00"),
                            "amount" => (Convert.ToDecimal(item.PayAmount - item.Fee) / 100).ToString("0.00"),
                            "round" => $"{value:0.00}%",
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
                var rows = result.Count + 3;
                for (int i = 1; i < rows; i++)
                {
                    worksheet.Row(i).Height = 25;
                }
                worksheet.Cells[1, 1, 1, cols].Style.Font.Size = 12;
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

        private void ChickAuth(string method)
        {
            if (method.Equals("transaction", StringComparison.OrdinalIgnoreCase) && !GetCurrentUser().Authorities.Contains(PermCode.MGR.ENT_STATISTIC_TRANSACTION))
            {
                throw new BizException("当前用户未分配该菜单权限！");
            }
            if (method.Equals("mch", StringComparison.OrdinalIgnoreCase) && !GetCurrentUser().Authorities.Contains(PermCode.MGR.ENT_STATISTIC_MCH))
            {
                throw new BizException("当前用户未分配该菜单权限！");
            }
        }
    }
}
