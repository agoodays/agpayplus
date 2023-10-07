using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
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

        public QueryOrderController(RequestKit requestKit,
            IPayOrderService payOrderService,
            ConfigContextQueryService configContextQueryService)
            : base(requestKit, configContextQueryService)
        {
            this.payOrderService = payOrderService;
        }

        [HttpPost]
        [Route("query")]
        public ActionResult<ApiRes> QueryOrder()
        {
            //获取参数 & 验签
            QueryPayOrderRQ rq = GetRQByWithMchSign<QueryPayOrderRQ>();

            if (StringUtil.IsAllNullOrWhiteSpace(rq.MchOrderNo, rq.PayOrderId))
            {
                throw new BizException("mchOrderNo 和 payOrderId不能同时为空");
            }

            PayOrderDto payOrder = payOrderService.QueryMchOrder(rq.MchNo, rq.PayOrderId, rq.MchOrderNo);
            if (payOrder == null)
            {
                throw new BizException("订单不存在");
            }

            QueryPayOrderRS bizRes = QueryPayOrderRS.BuildByPayOrder(payOrder);
            return ApiRes.OkWithSign(bizRes, rq.SignType, _configContextQueryService.QueryMchApp(rq.MchNo, rq.AppId).AppSecret);
        }
    }
}
