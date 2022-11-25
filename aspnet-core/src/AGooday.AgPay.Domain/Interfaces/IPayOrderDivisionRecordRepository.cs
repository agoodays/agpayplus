using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayOrderDivisionRecordRepository : IRepository<PayOrderDivisionRecord, long>
    {
        long SumSuccessDivisionAmount(string payOrderId);
    }
}
