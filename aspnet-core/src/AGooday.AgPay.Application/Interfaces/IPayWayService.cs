using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayWayService : IDisposable
    {
        bool Add(PayWayDto dto);
        bool Remove(string recordId);
        bool Update(PayWayDto dto);
        PayWayDto GetById(string recordId);
        bool IsExistPayWayCode(string wayCode);
        IEnumerable<PayWayDto> GetAll();
        PaginatedList<T> GetPaginatedData<T>(PayWayQueryDto dto);
    }
}
