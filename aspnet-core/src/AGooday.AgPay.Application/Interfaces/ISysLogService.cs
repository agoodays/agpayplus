using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysLogService : IAgPayService<SysLogDto, long>
    {
        Task<bool> RemoveByIdsAsync(List<long> recordIds);
        Task<PaginatedResult<SysLogDto>> GetPaginatedDataAsync(SysLogQueryDto dto);
    }
}
