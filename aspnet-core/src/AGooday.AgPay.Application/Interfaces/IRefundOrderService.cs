using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IRefundOrderService : IAgPayService<RefundOrderDto>
    {
        /// <summary>
        /// 查询商户订单
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="mchRefundNo"></param>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        RefundOrderDto QueryMchOrder(string mchNo, string mchRefundNo, string refundOrderId);
        PaginatedList<RefundOrderDto> GetPaginatedData(RefundOrderQueryDto dto);
        JObject Statistics(RefundOrderQueryDto dto);
        bool IsExistOrderByMchOrderNo(string mchNo, string mchRefundNo);
        bool IsExistRefundingOrder(string payOrderId);
        long SumSuccessRefundAmount(string payOrderId);
        /// <summary>
        /// 更新退款单状态 【退款单生成】 --》 【退款中】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderId"></param>
        /// <returns></returns>
        bool UpdateInit2Ing(string refundOrderId, string channelOrderId);
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款成功】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderId"></param>
        /// <returns></returns>
        bool UpdateIng2Success(string refundOrderId, string channelOrderId);
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款失败】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        bool UpdateIng2Fail(string refundOrderId, string channelOrderId, string channelErrCode, string channelErrMsg);
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款成功/退款失败】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="state"></param>
        /// <param name="channelOrderId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        bool UpdateIng2SuccessOrFail(string refundOrderId, byte state, string channelOrderId, string channelErrCode, string channelErrMsg);
        /// <summary>
        /// 更新退款单为 关闭状态
        /// </summary>
        /// <returns></returns>
        int UpdateOrderExpired();
    }
}
