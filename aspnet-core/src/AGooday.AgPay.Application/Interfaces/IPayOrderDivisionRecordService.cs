using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderDivisionRecordService : IDisposable
    {
        bool Add(PayOrderDivisionRecordDto dto);
        bool Remove(long recordId);
        bool Update(PayOrderDivisionRecordDto dto);
        PayOrderDivisionRecordDto GetById(long recordId);
        PayOrderDivisionRecordDto GetById(long recordId, string mchNo);
        IEnumerable<PayOrderDivisionRecordDto> GetByPayOrderId(string payOrderId);
        List<PayOrderDivisionRecordDto> GetByBatchOrderId(PayOrderDivisionRecordQueryDto dto);
        IEnumerable<PayOrderDivisionRecordDto> GetAll();
        PaginatedList<PayOrderDivisionRecordDto> GetPaginatedData(PayOrderDivisionRecordQueryDto dto);
        PaginatedList<PayOrderDivisionRecordDto> DistinctBatchOrderIdList(PayOrderDivisionRecordQueryDto dto);
        void UpdateRecordSuccessOrFailBySingleItem(long recordId, byte state, string channelRespResult);
        void UpdateRecordSuccessOrFail(List<PayOrderDivisionRecordDto> records, byte state, string channelBatchOrderId, string channelRespResult);
        void UpdateResendState(string payOrderId);
    }
}
