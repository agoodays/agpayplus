using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleEntRelaService : IDisposable
    {
        void Add(SysRoleEntRelaVM vm);
        void Remove(string recordId);
        void Update(SysRoleEntRelaVM vm);
        SysRoleEntRelaVM GetById(string recordId);
        IEnumerable<SysRoleEntRelaVM> GetAll();
    }
}
