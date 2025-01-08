using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderDivisionRecordService : IAgPayService<PayOrderDivisionRecordDto, long>
    {
        Task<PayOrderDivisionRecordDto> GetByIdAsync(long recordId, string mchNo);
        Task<PayOrderDivisionRecordDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo);
        IEnumerable<PayOrderDivisionRecordDto> GetByPayOrderId(string payOrderId);
        List<PayOrderDivisionRecordDto> GetByBatchOrderId(PayOrderDivisionRecordQueryDto dto);
        Task<PaginatedList<PayOrderDivisionRecordDto>> GetPaginatedDataAsync(PayOrderDivisionRecordQueryDto dto);
        PaginatedList<PayOrderDivisionRecordDto> DistinctBatchOrderIdList(PayOrderDivisionRecordQueryDto dto);
        Task<bool> UpdateRecordSuccessOrFailBySingleItemAsync(long recordId, byte state, string channelRespResult);
        Task<int> UpdateRecordSuccessOrFailAsync(List<PayOrderDivisionRecordDto> records, byte state, string channelBatchOrderId, string channelRespResult);
        Task<bool> UpdateResendStateAsync(string payOrderId);
    }
}
