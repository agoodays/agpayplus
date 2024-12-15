using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 分账结果回调接口
    /// </summary>
    public interface IDivisionRecordChannelNotifyService
    {
        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 解析参数： 批次号 和 请求参数
        /// 异常需要自行捕捉，并返回null , 表示已响应数据。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request);

        /// <summary>
        /// 返回需要更新的记录 <ID, 结果> 状态 和响应数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="parameters"></param>
        /// <param name="recordList"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        Task<DivisionChannelNotifyModel> DoNotifyAsync(HttpRequest request, object parameters, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext);

        ActionResult DoNotifyOrderNotExists(HttpRequest request);

        ActionResult DoNotifyOrderStateUpdateFail(HttpRequest request);
    }
}
