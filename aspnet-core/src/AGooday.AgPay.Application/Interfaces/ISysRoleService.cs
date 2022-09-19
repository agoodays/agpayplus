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
        void Add(SysRoleDto dto);
        void Remove(string recordId);
        void Update(SysRoleDto dto);
        SysRoleDto GetById(string recordId);
        IEnumerable<SysRoleDto> GetAll();
    }
}
