using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderService : IDisposable
    {
        void Add(PayOrderVM vm);
        void Remove(string recordId);
        void Update(PayOrderVM vm);
        PayOrderVM GetById(string recordId);
        IEnumerable<PayOrderVM> GetAll();
    }
}
