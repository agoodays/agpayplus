using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 调起上游渠道侧支付接口
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 是否支持该支付方式
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        bool IsSupport(string wayCode);

        /// <summary>
        /// 前置检查如参数等信息是否符合要求， 返回错误信息或直接抛出异常即可
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder);

        /// <summary>
        /// 计算手续费
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        long CalculateFeeAmount(long amount, decimal rate);

        /// <summary>
        /// 计算分润金额
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        long CalculateProfitAmount(long amount, decimal rate);

        /// <summary>
        /// 调起支付接口，并响应数据；  内部处理普通商户和服务商模式
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <param name="payOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext);
    }
}
