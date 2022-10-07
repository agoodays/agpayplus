using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using System;

namespace AGooday.AgPay.Payment.Api.Channel
{
    public interface IDivisionService
    {
        /** 获取到接口code **/
        string GetIfCode();

        /** 是否支持该分账 */
        bool IsSupport();

        /** 绑定关系 **/
        ChannelRetMsg Bind(MchDivisionReceiverDto mchDivisionReceiver, MchAppConfigContext mchAppConfigContext);

        /** 单次分账 （无需调用完结接口，或自动解冻商户资金)  **/
        ChannelRetMsg SingleDivision(PayOrderDto payOrder, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext);
    }
}
