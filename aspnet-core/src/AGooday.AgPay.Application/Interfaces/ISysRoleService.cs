using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleService : IAgPayService<SysRoleDto>
    {
        Task AddAsync(SysRoleCreateDto dto);
        Task UpdateAsync(SysRoleModifyDto dto);
        Task RemoveRoleAsync(string roleId);
        Task<SysRoleDto> GetByIdAsync(string recordId, string belongInfoId);
        Task<PaginatedList<SysRoleDto>> GetPaginatedDataAsync(SysRoleQueryDto dto);
    }
}
