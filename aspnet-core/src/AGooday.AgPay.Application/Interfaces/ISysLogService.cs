using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysLogService : IDisposable
    {
        void Add(SysLogVM vm);
        void Remove(long recordId);
        void Update(SysLogVM vm);
        SysLogVM GetById(long recordId);
        IEnumerable<SysLogVM> GetAll();
    }
}
