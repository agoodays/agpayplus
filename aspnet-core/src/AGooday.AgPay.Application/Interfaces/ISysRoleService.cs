using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleService : IDisposable
    {
        void Add(SysRoleCreateDto dto);
        void Remove(string recordId);
        void Update(SysRoleModifyDto dto);
        SysRoleDto GetById(string recordId);
        SysRoleDto GetById(string recordId, string belongInfoId);
        IEnumerable<SysRoleDto> GetAll();
        PaginatedList<SysRoleDto> GetPaginatedData(SysRoleQueryDto dto);
        void RemoveRole(string roleId);
    }
}
