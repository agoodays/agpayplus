using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayOrderDivisionRecordRepository : IAgPayRepository<PayOrderDivisionRecord, long>
    {
        long SumSuccessDivisionAmount(string payOrderId);
    }
}
