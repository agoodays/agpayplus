using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderProfitService : IDisposable
    {
        bool Add(PayOrderProfitDto dto);
        bool Remove(long recordId);
        bool Update(PayOrderProfitDto dto);
        PayOrderProfitDto GetById(long recordId);
        IEnumerable<PayOrderProfitDto> GetAll();
        IEnumerable<PayOrderProfitDto> GetByPayOrderIdAsNoTracking(string payOrderId);
        IEnumerable<PayOrderProfitDto> GetByPayOrderIdsAsNoTracking(List<string> payOrderIds);
    }
}
