using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 调起上游渠道侧退款接口
    /// </summary>
    public interface IRefundService
    {
        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 前置检查如参数等信息是否符合要求， 返回错误信息或直接抛出异常即可
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <param name="refundOrder"></param>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder);

        /// <summary>
        /// 调起退款接口，并响应数据；  内部处理普通商户和服务商模式
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <param name="refundOrder"></param>
        /// <param name="payOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        ChannelRetMsg Refund(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext);

        /// <summary>
        /// 退款查单接口
        /// </summary>
        /// <param name="refundOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext);
    }
}
