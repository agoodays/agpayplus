using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayOrderDivisionRecordRepository : IAgPayRepository<PayOrderDivisionRecord, long>
    {
        Task<PayOrderDivisionRecord> GetByIdAsNoTrackingAsync(long recordId, string mchNo);
        Task<long> SumSuccessDivisionAmountAsync(string payOrderId);
    }
}
