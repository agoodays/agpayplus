using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 商户查单
    /// </summary>
    [ApiController]
    [Route("api/pay")]
    public class QueryOrderController : ApiControllerBase
    {
        private readonly IPayOrderService payOrderService;

        public QueryOrderController(RequestIpUtil requestIpUtil,
            IPayOrderService payOrderService,
            ConfigContextQueryService configContextQueryService)
            : base(requestIpUtil, configContextQueryService)
        {
            this.payOrderService = payOrderService;
        }

        [HttpPost]
        [Route("query")]
        public ActionResult<ApiRes> QueryOrder()
        {
            //获取参数 & 验签
            QueryPayOrderRQ rq = GetRQByWithMchSign<QueryPayOrderRQ>();

            if (string.IsNullOrWhiteSpace(rq.MchOrderNo) && string.IsNullOrWhiteSpace(rq.PayOrderId))
            {
                throw new BizException("mchOrderNo 和 payOrderId不能同时为空");
            }

            PayOrderDto payOrder = payOrderService.QueryMchOrder(rq.MchNo, rq.PayOrderId, rq.MchOrderNo);
            if (payOrder == null)
            {
                throw new BizException("订单不存在");
            }

            QueryPayOrderRS bizRes = QueryPayOrderRS.BuildByPayOrder(payOrder);
            return ApiRes.OkWithSign(bizRes, _configContextQueryService.QueryMchApp(rq.MchNo, rq.AppId).AppSecret);
        }
    }
}
