using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceDefineService : IDisposable
    {
        void Add(PayInterfaceDefineVM vm);
        void Remove(string recordId);
        void Update(PayInterfaceDefineVM recordId);
        PayInterfaceDefineVM GetById(string recordId);
        IEnumerable<PayInterfaceDefineVM> GetAll();
    }
}
