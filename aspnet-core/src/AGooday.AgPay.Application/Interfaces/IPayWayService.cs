using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayWayService : IDisposable
    {
        void Add(PayWayVM vm);
        void Remove(string recordId);
        void Update(PayWayVM vm);
        PayWayVM GetById(string recordId);
        IEnumerable<PayWayVM> GetAll();
    }
}
