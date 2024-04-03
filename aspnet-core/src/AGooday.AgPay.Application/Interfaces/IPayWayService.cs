using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayWayService : IAgPayService<PayWayDto>
    {
        string GetWayTypeByWayCode(string wayCode);
        bool IsExistPayWayCode(string wayCode);
        PaginatedList<T> GetPaginatedData<T>(PayWayQueryDto dto);
    }
}
