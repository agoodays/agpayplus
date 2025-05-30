using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleService : IAgPayService<SysRoleDto>
    {
        Task<bool> AddAsync(SysRoleCreateDto dto);
        Task<bool> UpdateAsync(SysRoleModifyDto dto);
        Task<bool> RemoveRoleAsync(string roleId);
        Task<SysRoleDto> GetByIdAsNoTrackingAsync(string recordId, string belongInfoId);
        Task<PaginatedList<SysRoleDto>> GetPaginatedDataAsync(SysRoleQueryDto dto);
    }
}
