using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
