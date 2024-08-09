using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Refund
{
    /// <summary>
    /// 商户退款单查询
    /// </summary>
    [ApiController]
    public class QueryRefundOrderController : ApiControllerBase
    {
        protected readonly ILogger<QueryRefundOrderController> _logger;
        private readonly IRefundOrderService _refundOrderService;

        public QueryRefundOrderController(ILogger<QueryRefundOrderController> logger,
            IRefundOrderService refundOrderService,
            ConfigContextQueryService configContextQueryService,
            RequestKit requestKit)
            : base(requestKit, configContextQueryService)
        {
            _logger = logger;
            _refundOrderService = refundOrderService;
        }

        [HttpPost, Route("api/refund/query")]
        [PermissionAuth(PermCode.PAY.API_REFUND_ORDER_QUERY)]
        public ApiRes QueryRefundOrder()
        {
            //获取参数 & 验签
            QueryRefundOrderRQ rq = GetRQByWithMchSign<QueryRefundOrderRQ>();

            if (StringUtil.IsAllNullOrWhiteSpace(rq.MchRefundNo, rq.RefundOrderId))
            {
                throw new BizException("mchRefundNo 和 refundOrderId不能同时为空");
            }

            RefundOrderDto refundOrder = _refundOrderService.QueryMchOrder(rq.MchNo, rq.MchRefundNo, rq.RefundOrderId);
            if (refundOrder == null)
            {
                throw new BizException("订单不存在");
            }

            QueryRefundOrderRS bizRes = QueryRefundOrderRS.BuildByRefundOrder(refundOrder);
            return ApiRes.OkWithSign(bizRes, rq.SignType, _configContextQueryService.QueryMchApp(rq.MchNo, rq.AppId).AppSecret);
        }
    }
}
