using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysLogService : IAgPayService<SysLogDto, long>
    {
        bool RemoveByIds(List<long> recordIds);
        Task<PaginatedList<SysLogDto>> GetPaginatedDataAsync(SysLogQueryDto dto);
    }
}
