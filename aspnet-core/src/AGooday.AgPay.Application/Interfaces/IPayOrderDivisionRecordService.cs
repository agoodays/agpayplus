using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderDivisionRecordService : IAgPayService<PayOrderDivisionRecordDto, long>
    {
        PayOrderDivisionRecordDto GetById(long recordId, string mchNo);
        Task<PayOrderDivisionRecordDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo);
        IEnumerable<PayOrderDivisionRecordDto> GetByPayOrderId(string payOrderId);
        List<PayOrderDivisionRecordDto> GetByBatchOrderId(PayOrderDivisionRecordQueryDto dto);
        PaginatedList<PayOrderDivisionRecordDto> GetPaginatedData(PayOrderDivisionRecordQueryDto dto);
        PaginatedList<PayOrderDivisionRecordDto> DistinctBatchOrderIdList(PayOrderDivisionRecordQueryDto dto);
        void UpdateRecordSuccessOrFailBySingleItem(long recordId, byte state, string channelRespResult);
        void UpdateRecordSuccessOrFail(List<PayOrderDivisionRecordDto> records, byte state, string channelBatchOrderId, string channelRespResult);
        void UpdateResendState(string payOrderId);
    }
}
