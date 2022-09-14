using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchAppService : IDisposable
    {
        void Add(MchAppVM vm);
        void Remove(string recordId);
        void Update(MchAppVM recordId);
        MchAppVM GetById(string recordId);
        IEnumerable<MchAppVM> GetAll();
    }
}
