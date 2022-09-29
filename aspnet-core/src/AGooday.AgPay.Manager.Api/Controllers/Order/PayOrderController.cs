using AGooday.AgPay.AopSdk;
using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Manager.Api.Controllers.Order
{
    /// <summary>
    /// 支付订单类
    /// </summary>
    [Route("/api/payOrder")]
    [ApiController]
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
        [HttpGet, Route("")]
        public ApiRes List([FromQuery] PayOrderQueryDto dto)
        {
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
        /// 支付订单信息
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <returns></returns>
        [HttpGet, Route("{payOrderId}")]
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
        /// <param name="refundAmount"></param>
        /// <param name="refundReason"></param>
        /// <returns></returns>
        [HttpGet, Route("refunds/{payOrderId}")]
        public ApiRes Refund(string payOrderId, long refundAmount, string refundReason)
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
            if (payOrder.RefundAmount + refundAmount > payOrder.Amount)
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
            model.RefundAmount = refundAmount;
            model.RefundReason = refundReason;
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
