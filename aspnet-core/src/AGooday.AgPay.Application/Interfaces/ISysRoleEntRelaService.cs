using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleEntRelaService : IDisposable
    {
        void Add(SysRoleEntRelaDto dto);
        void Remove(string recordId);
        void Update(SysRoleEntRelaDto dto);
        SysRoleEntRelaDto GetById(string recordId);
        IEnumerable<SysRoleEntRelaDto> GetAll();
    }
}
