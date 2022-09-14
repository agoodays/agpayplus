using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleService : IDisposable
    {
        void Add(SysRoleVM vm);
        void Remove(string recordId);
        void Update(SysRoleVM recordId);
        SysRoleVM GetById(string recordId);
        IEnumerable<SysRoleVM> GetAll();
    }
}
