using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleService : IAgPayService<SysRoleDto>
    {
        void Add(SysRoleCreateDto dto);
        void Update(SysRoleModifyDto dto);
        void RemoveRole(string roleId);
        SysRoleDto GetById(string recordId, string belongInfoId);
        PaginatedList<SysRoleDto> GetPaginatedData(SysRoleQueryDto dto);
    }
}
