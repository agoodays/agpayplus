using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysEntitlementService : IDisposable
    {
        void Add(SysEntitlementVM vm);
        void Remove(string recordId);
        void Update(SysEntitlementVM vm);
        SysEntitlementVM GetById(string recordId);
        IEnumerable<SysEntitlementVM> GetBySysType(string sysType, string entId);
        IEnumerable<SysEntitlementVM> GetAll();
    }
}
