using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysConfigService : IDisposable
    {
        void Add(SysConfigVM vm);
        void Remove(string recordId);
        void Update(SysConfigVM vm);
        SysConfigVM GetById(string recordId);
        IEnumerable<SysConfigVM> GetAll();
    }
}
