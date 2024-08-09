﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Authorization;
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

        public QueryOrderController(IPayOrderService payOrderService,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(requestKit, configContextQueryService)
        {
            this.payOrderService = payOrderService;
        }

        [HttpPost, Route("query")]
        [PermissionAuth(PermCode.PAY.API_PAY_ORDER_QUERY)]
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
