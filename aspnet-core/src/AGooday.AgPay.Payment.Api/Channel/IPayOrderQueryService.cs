using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 查单（渠道侧）接口定义
    /// </summary>
    public interface IPayOrderQueryService
    {
        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="payOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext);
    }
}
