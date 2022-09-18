using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceConfigService : IDisposable
    {
        void Add(PayInterfaceConfigVM vm);
        void Remove(long recordId);
        void Update(PayInterfaceConfigVM vm);
        PayInterfaceConfigVM GetById(long recordId);
        IEnumerable<PayInterfaceConfigVM> GetAll();
    }
}
