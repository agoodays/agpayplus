using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayWayService : IAgPayService<PayWayDto>
    {
        Task<bool> IsExistPayWayCodeAsync(string wayCode);
        Task<string> GetWayTypeByWayCodeAsync(string wayCode);
        Task<PaginatedList<T>> GetPaginatedDataAsync<T>(PayWayQueryDto dto);
    }
}
