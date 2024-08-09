using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 关闭订单（渠道侧）接口定义
    /// </summary>
    public interface IPayOrderCloseService
    {
        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="payOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        ChannelRetMsg Close(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext);
    }
}
