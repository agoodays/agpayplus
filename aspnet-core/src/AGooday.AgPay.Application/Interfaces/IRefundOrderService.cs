using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IRefundOrderService : IDisposable
    {
        void Add(RefundOrderDto dto);
        void Remove(string recordId);
        void Update(RefundOrderDto dto);
        RefundOrderDto GetById(string recordId);
        RefundOrderDto QueryMchOrder(string mchNo, string mchRefundNo, string refundOrderId);
        IEnumerable<RefundOrderDto> GetAll();
        PaginatedList<RefundOrderDto> GetPaginatedData(RefundOrderQueryDto dto);
        long SumSuccessRefundAmount(string payOrderId);
        bool IsExistOrderByMchOrderNo(string mchNo, string mchRefundNo);
        bool IsExistRefundingOrder(string payOrderId);
        bool UpdateInit2Ing(string refundOrderId);
        bool UpdateIng2Success(string refundOrderId, string channelOrderId);
        bool UpdateIng2Fail(string refundOrderId, string channelOrderId, string channelErrCode, string channelErrMsg);
        bool UpdateIng2SuccessOrFail(string refundOrderId, byte state, string channelOrderId, string channelErrCode, string channelErrMsg);
    }
}
