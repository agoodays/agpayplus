using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Transfer;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 转账接口
    /// </summary>
    public interface ITransferService
    {
        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 是否支持该支付入账方式
        /// </summary>
        /// <param name="entryType"></param>
        /// <returns></returns>
        bool IsSupport(string entryType);

        /// <summary>
        /// 前置检查如参数等信息是否符合要求， 返回错误信息或直接抛出异常即可
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <param name="transferOrder"></param>
        /// <returns></returns>
        string PreCheck(TransferOrderRQ bizRQ, TransferOrderDto transferOrder);

        /// <summary>
        /// 调起退款接口，并响应数据；  内部处理普通商户和服务商模式
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <param name="transferOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        ChannelRetMsg Transfer(TransferOrderRQ bizRQ, TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext);

        /// <summary>
        /// 调起转账查询接口
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        ChannelRetMsg Query(TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext);
    }
}
