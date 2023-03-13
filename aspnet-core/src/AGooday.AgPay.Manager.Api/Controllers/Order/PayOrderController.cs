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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Net;

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
            return ApiRes.Ok(new
            {
                allPayAmount = 590766239 / 100.00,
                allPayCount = 1871,
                failPayAmount = 590714131 / 100.00,
                failPayCount = 1691,
                mchFeeAmount = 6097 / 100.00,
                payAmount = 52108 / 100.00,
                payCount = 180,
                refundAmount = 16635 / 100.00,
                refundCount = 45
            });
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
            // 创建新的 Excel 文件
            using (var package = new ExcelPackage())
            {
                // 添加工作表，并设置标题行
                var worksheet = package.Workbook.Worksheets.Add("订单列表");
                worksheet.Cells["A1"].Value = $"订单列表";
                // 设置单元格样式，例如居中对齐和加粗字体
                worksheet.Cells["A1:O1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:O1"].Style.Font.Bold = true;
                worksheet.Cells["A2"].Value = $"支付订单号";
                worksheet.Cells["B2"].Value = $"商户订单号";
                worksheet.Cells["C2"].Value = $"商户号";
                worksheet.Cells["D2"].Value = $"商户名称";
                worksheet.Cells["E2"].Value = $"门店ID";
                worksheet.Cells["F2"].Value = $"门店名称";
                worksheet.Cells["G2"].Value = $"支付状态";
                worksheet.Cells["H2"].Value = $"退款状态";
                worksheet.Cells["I2"].Value = $"服务商号";
                worksheet.Cells["J2"].Value = $"支付方式";
                worksheet.Cells["K2"].Value = $"支付金额";
                worksheet.Cells["L2"].Value = $"退款金额";
                worksheet.Cells["M2"].Value = $"手续费";
                worksheet.Cells["N2"].Value = $"创建时间";
                worksheet.Cells["O2"].Value = $"支付成功时间";
                // 将每个订单添加到工作表中
                for (int i = 0; i < payOrders.Count(); i++)
                {
                    var order = payOrders[i];
                    worksheet.Cells[$"A{i + 3}"].Value = order.PayOrderId;
                    worksheet.Cells[$"B{i + 3}"].Value = order.MchOrderNo;
                    worksheet.Cells[$"C{i + 3}"].Value = order.MchNo;
                    worksheet.Cells[$"D{i + 3}"].Value = order.MchName;
                    worksheet.Cells[$"E{i + 3}"].Value = order.StoreId;
                    worksheet.Cells[$"F{i + 3}"].Value = order.StoreName;
                    worksheet.Cells[$"G{i + 3}"].Value = order.State;
                    worksheet.Cells[$"H{i + 3}"].Value = order.RefundState;
                    worksheet.Cells[$"I{i + 3}"].Value = order.IsvNo;
                    worksheet.Cells[$"J{i + 3}"].Value = order.WayName;
                    worksheet.Cells[$"K{i + 3}"].Value = order.Amount / 100.00;
                    worksheet.Cells[$"L{i + 3}"].Value = order.RefundAmount / 100.00;
                    worksheet.Cells[$"M{i + 3}"].Value = order.MchFeeAmount / 100.00;
                    worksheet.Cells[$"N{i + 3}"].Value = order.CreatedAt;
                    worksheet.Cells[$"O{i + 3}"].Value = order.SuccessTime;
                }
                // 设置响应头，指示将要下载的文件类型为 Excel 文件
                Response.Headers.Add("Content-Disposition", $"attachment;filename=\"{WebUtility.UrlEncode(fileName)}\"");
                Response.ContentType = "application/vnd.ms-excel;charset=UTF-8";
                // 将 Excel 文件写入 HTTP 响应流中，并返回给客户端
                return File(package.GetAsByteArray(), Response.ContentType, fileName);
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
