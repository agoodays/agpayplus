﻿using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder.PayWay
{
    /// <summary>
    /// 支付宝 jspai
    /// </summary>
    [ApiController]
    public class AliJsapiOrderController : AbstractPayOrderController
    {
        /// <summary>
        /// 统一下单接口
        /// </summary>
        [HttpPost]
        [Route("api/pay/aliJsapiOrder")]
        public ActionResult<ApiRes> AliJsapiOrder(AliJsapiOrderRQ bizRQ)
        {
            // 统一下单接口
            return UnifiedOrder(CS.PAY_WAY_CODE.ALI_JSAPI, bizRQ);// "ALI_JSAPI";  //支付宝服务窗支付
        }
    }
}