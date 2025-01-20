using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IRefundOrderService : IAgPayService<RefundOrderDto>
    {
        Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchRefundNo);
        Task<bool> IsExistRefundingOrderAsync(string payOrderId);
        /// <summary>
        /// 查询商户订单
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="mchRefundNo"></param>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        Task<RefundOrderDto> QueryMchOrderAsync(string mchNo, string mchRefundNo, string refundOrderId);
        Task<PaginatedList<RefundOrderDto>> GetPaginatedDataAsync(RefundOrderQueryDto dto);
        Task<JObject> StatisticsAsync(RefundOrderQueryDto dto);
        Task<long> SumSuccessRefundAmountAsync(string payOrderId);
        /// <summary>
        /// 更新退款单状态 【退款单生成】 --》 【退款中】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderId"></param>
        /// <returns></returns>
        Task<bool> UpdateInit2IngAsync(string refundOrderId, string channelOrderId);
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款成功】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderId"></param>
        /// <returns></returns>
        Task<bool> UpdateIng2SuccessAsync(string refundOrderId, string channelOrderId);
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款失败】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        Task<bool> UpdateIng2FailAsync(string refundOrderId, string channelOrderId, string channelErrCode, string channelErrMsg);
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款成功/退款失败】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="state"></param>
        /// <param name="channelOrderId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        Task<bool> UpdateIng2SuccessOrFailAsync(string refundOrderId, byte state, string channelOrderId, string channelErrCode, string channelErrMsg);
        /// <summary>
        /// 更新退款单为 关闭状态
        /// </summary>
        /// <returns></returns>
        Task<int> UpdateOrderExpiredAsync();
        /// <summary>
        /// 更新支付订单分润并生成账单
        /// </summary>
        /// <param name="payOrderProfitDtos"></param>
        /// <param name="accountBillDtos"></param>
        /// <returns></returns>
        Task<int> UpdatePayOrderProfitAndGenAccountBillAsync(List<PayOrderProfitDto> payOrderProfitDtos, List<AccountBillDto> accountBillDtos);
    }
}
