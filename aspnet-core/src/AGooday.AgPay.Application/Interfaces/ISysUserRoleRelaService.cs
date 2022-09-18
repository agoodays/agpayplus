using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserRoleRelaService : IDisposable
    {
        void Add(SysUserRoleRelaVM vm);
        void Remove(string recordId);
        void Update(SysUserRoleRelaVM vm);
        SysUserRoleRelaVM GetById(string recordId);
        IEnumerable<SysUserRoleRelaVM> GetAll();
    }
}
