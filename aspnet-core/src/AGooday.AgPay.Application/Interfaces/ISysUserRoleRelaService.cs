using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserRoleRelaService : IDisposable
    {
        void Add(SysUserRoleRelaDto dto);
        void Remove(string recordId);
        void Update(SysUserRoleRelaDto dto);
        SysUserRoleRelaDto GetById(string recordId);
        IEnumerable<string> SelectRoleIdsByUserId(long userId);
        IEnumerable<long> SelectRoleIdsByRoleId(string roleId);
        IEnumerable<SysUserRoleRelaDto> GetAll();
        PaginatedList<SysUserRoleRelaDto> GetPaginatedData(SysUserRoleRelaDto dto, int pageIndex = 1, int pageSize = 20);
    }
}
