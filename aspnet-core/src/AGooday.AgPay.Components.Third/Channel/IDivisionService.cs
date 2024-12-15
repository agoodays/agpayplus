using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 分账接口
    /// </summary>
    public interface IDivisionService
    {
        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 是否支持该分账
        /// </summary>
        /// <returns></returns>
        bool IsSupport();

        /// <summary>
        /// 绑定关系
        /// </summary>
        /// <param name="mchDivisionReceiver"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        Task<ChannelRetMsg> BindAsync(MchDivisionReceiverDto mchDivisionReceiver, MchAppConfigContext mchAppConfigContext);

        /// <summary>
        /// 单次分账 （无需调用完结接口，或自动解冻商户资金)
        /// </summary>
        /// <param name="payOrder"></param>
        /// <param name="recordList"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        Task<ChannelRetMsg> SingleDivisionAsync(PayOrderDto payOrder, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext);

        /// <summary>
        /// 查询分账结果
        /// </summary>
        /// <param name="payOrder"></param>
        /// <param name="recordList"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        Task<Dictionary<long, ChannelRetMsg>> QueryDivisionAsync(PayOrderDto payOrder, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext);
    }
}
