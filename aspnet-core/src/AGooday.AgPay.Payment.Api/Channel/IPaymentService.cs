using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using System;
using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 调起上游渠道侧支付接口
    /// </summary>
    public interface IPaymentService
    {
        /** 获取到接口code **/
        string GetIfCode();

        /** 是否支持该支付方式 */
        bool IsSupport(string wayCode);

        /** 前置检查如参数等信息是否符合要求， 返回错误信息或直接抛出异常即可  */
        string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder);

        /** 调起支付接口，并响应数据；  内部处理普通商户和服务商模式  **/
        AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext);
    }
}
