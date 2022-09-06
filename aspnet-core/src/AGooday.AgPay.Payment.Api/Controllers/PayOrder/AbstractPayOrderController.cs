using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 创建支付订单抽象类
    /// </summary>
    public abstract class AbstractPayOrderController : ApiControllerBase
    {
        /// <summary>
        /// 统一下单 (新建订单模式)
        /// </summary>
        /// <param name="wayCode"></param>
        /// <param name="bizRQ">业务请求报文</param>
        /// <returns></returns>
        protected ResultBase UnifiedOrder(string wayCode, UnifiedOrderRQ bizRQ)
        {
            return UnifiedOrder(wayCode, bizRQ, null);
        }

        /// <summary>
        /// 统一下单
        /// </summary>
        /// <param name="wayCode"></param>
        /// <param name="bizRQ"></param>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        protected ResultBase UnifiedOrder(string wayCode, UnifiedOrderRQ bizRQ, Models.PayOrder payOrder)
        {
            return ResultBase.Ok(bizRQ);
        }
    }
}
