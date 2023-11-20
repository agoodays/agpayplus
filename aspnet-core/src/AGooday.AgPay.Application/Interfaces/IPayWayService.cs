using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayWayService : IDisposable
    {
        bool Add(PayWayDto dto);
        bool Remove(string recordId);
        bool Update(PayWayDto dto);
        PayWayDto GetById(string recordId);
        string GetWayTypeByWayCode(string wayCode);
        bool IsExistPayWayCode(string wayCode);
        IEnumerable<PayWayDto> GetAll();
        PaginatedList<T> GetPaginatedData<T>(PayWayQueryDto dto);
    }
}
