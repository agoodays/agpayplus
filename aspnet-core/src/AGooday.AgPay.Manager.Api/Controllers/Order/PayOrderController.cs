using AGooday.AgPay.AopSdk;
using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AGooday.AgPay.Manager.Api.Controllers.Order
{
    /// <summary>
    /// 支付订单类
    /// </summary>
    [Route("/api/payOrder")]
    [ApiController, Authorize]
    public class PayOrderController : ControllerBase
    {
        private readonly ILogger<PayOrderController> _logger;
        private readonly IPayOrderService _payOrderService;
        private readonly IPayWayService _payWayService;
        private readonly ISysConfigService _sysConfigService;
        private readonly IMchAppService _mchAppService;

        public PayOrderController(ILogger<PayOrderController> logger,
            IPayOrderService payOrderService,
            IPayWayService payWayService,
            ISysConfigService sysConfigService,
            IMchAppService mchAppService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
            _payWayService = payWayService;
            _sysConfigService = sysConfigService;
            _mchAppService = mchAppService;
        }

        /// <summary>
        /// 订单信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_LIST)]
        public ApiRes List([FromQuery] PayOrderQueryDto dto)
        {
            dto.BindDateRange();
            var payOrders = _payOrderService.GetPaginatedData(dto);
            // 得到所有支付方式
            Dictionary<string, string> payWayNameMap = new Dictionary<string, string>();
            _payWayService.GetAll().Select(s => new { s.WayCode, s.WayName }).ToList().ForEach((c) =>
            {
                payWayNameMap.Add(c.WayCode, c.WayName);
            });

            foreach (var payOrder in payOrders)
            {
                // 存入支付方式名称
                payOrder.WayName = payWayNameMap.ContainsKey(payOrder.WayCode) ? payWayNameMap[payOrder.WayCode] : payOrder.WayCode;
            }
            return ApiRes.Ok(new { Records = payOrders.ToList(), Total = payOrders.TotalCount, Current = payOrders.PageIndex, HasNext = payOrders.HasNext });
        }

        /// <summary>
        /// 订单信息统计
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("count"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_LIST)]
        public ApiRes Count([FromQuery] PayOrderQueryDto dto)
        {
            dto.BindDateRange();
            var statistics = _payOrderService.Statistics(dto);
            return ApiRes.Ok(statistics);
        }

        /// <summary>
        /// 订单信息导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("export/{bizType}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ORDER_LIST)]
        public IActionResult Export(string bizType, [FromQuery] PayOrderQueryDto dto)
        {
            dto.BindDateRange();
            // 从数据库中检索需要导出的数据
            var payOrders = _payOrderService.GetPaginatedData(dto);

            string fileName = $"订单列表.xlsx";
            // 5.0之后的epplus需要指定 商业证书 或者非商业证书。低版本不需要此行代码
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<dynamic> excelHeaders = new List<dynamic>() {
                new { Key = "payOrderId", Width = 30d, Value = $"支付订单号" },
                new { Key = "mchOrderNo", Width = 26d, Value = $"商户订单号" },
                new { Key = "mchNo", Width = 25d, Value = $"商户号" },
                new { Key = "mchName", Width = 25d, Value = $"商户名称" },
                new { Key = "storeId", Width = 15d, Value = $"门店ID" },
                new { Key = "storeName", Width = 22d, Value = $"门店名称" },
                new { Key = "state", Width = 10d, Value = $"支付状态" },
                new { Key = "refundState", Width = 10d, Value = $"退款状态" },
                new { Key = "isvNo", Width = 25d, Value = $"服务商号" },
                new { Key = "wayName", Width = 12d, Value = $"支付方式" },
                new { Key = "amount", Width = 10d, Value = $"支付金额" },
                new { Key = "refundAmount", Width = 10d, Value = $"退款金额" },
                new { Key = "mchFeeAmount", Width = 10d, Value = $"手续费" },
                new { Key = "createdAt", Width = 23d, Value = $"创建时间" },
                new { Key = "successTime", Width = 23d, Value = $"支付成功时间" }
            };
            // 创建新的 Excel 文件
            using (var package = new ExcelPackage())
            {
                // 添加工作表，并设置标题行
                var worksheet = package.Workbook.Worksheets.Add("订单列表");
                worksheet.Cells[1, 1].Value = $"订单列表";

                for (int i = 0; i < excelHeaders.Count; i++)
                {
                    var excelHeader = excelHeaders[i];
                    worksheet.Cells[2, i + 1].Value = excelHeader.Value;
                    worksheet.Column(i + 1).Width = excelHeader.Width;
                }
                // 固定前两行，第一列，`FreezePanes()`方法的第一个参数设置为3，表示从第三行开始向下滚动时会被冻结，第二个参数设置为3，表示从第二行开始向右滚动时会被冻结
                worksheet.View.FreezePanes(3, 2);
                // 将每个订单添加到工作表中
                for (int i = 0; i < payOrders.Count(); i++)
                {
                    var order = payOrders[i];
                    var orderJO = JObject.FromObject(order);
                    for (int j = 0; j < excelHeaders.Count; j++)
                    {
                        var excelHeader = excelHeaders[j];
                        var value = orderJO[excelHeader.Key];
                        switch (excelHeader.Key)
                        {
                            case "state":
                                value = order.State == 0 ? "订单生成" : order.State == 1 ? "支付中" : order.State == 2 ? "支付成功" : order.State == 3 ? "支付失败" : order.State == 4 ? "已撤销" : order.State == 5 ? "已退款" : order.State == 6 ? "订单关闭" : "未知";
                                break;
                            case "amount":
                            case "refundAmount":
                            case "mchFeeAmount":
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
                var rows = payOrders.Count() + 3;
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
        /// 支付订单信息
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <returns></returns>
        [HttpGet, Route("{payOrderId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_PAY_ORDER_VIEW)]
        public ApiRes Detail(string payOrderId)
        {
            var payOrder = _payOrderService.GetById(payOrderId);
            if (payOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(payOrder);
        }

        /// <summary>
        /// 发起订单退款
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="refundOrder"></param>
        /// <returns></returns>
        [HttpPost, Route("refunds/{payOrderId}"), MethodLog("发起订单退款")]
        [PermissionAuth(PermCode.MGR.ENT_PAY_ORDER_REFUND)]
        public ApiRes Refund(string payOrderId, RefundOrderModel refundOrder)
        {
            var payOrder = _payOrderService.GetById(payOrderId);
            if (payOrder == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (payOrder.State != (byte)PayOrderState.STATE_SUCCESS)
            {
                throw new BizException("订单状态不正确");
            }
            if (payOrder.RefundAmount + refundOrder.RefundAmount > payOrder.Amount)
            {
                throw new BizException("退款金额超过订单可退款金额！");
            }

            //发起退款
            RefundOrderCreateRequest request = new RefundOrderCreateRequest();
            RefundOrderCreateReqModel model = new RefundOrderCreateReqModel();
            model.MchNo = payOrder.MchNo;// 商户号
            model.AppId = payOrder.AppId;
            model.PayOrderId = payOrderId;
            model.MchRefundNo = SeqUtil.GenMhoOrderId();
            model.RefundAmount = refundOrder.RefundAmount;
            model.RefundReason = refundOrder.RefundReason;
            model.Currency = "CNY";
            request.SetBizModel(model);

            var mchApp = _mchAppService.GetById(payOrder.AppId);

            var agpayClient = new AgPayClient(_sysConfigService.GetDBApplicationConfig().PaySiteUrl, mchApp.AppSecret);
            try
            {
                var response = agpayClient.Execute(request);
                if (response.code != 0)
                {
                    throw new BizException(response.msg);
                }
                return ApiRes.Ok(response);
            }
            catch (AgPayException e)
            {
                throw new BizException(e.Message);
            }
        }
    }
}
