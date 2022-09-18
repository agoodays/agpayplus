using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IRefundOrderService : IDisposable
    {
        void Add(RefundOrderVM vm);
        void Remove(string recordId);
        void Update(RefundOrderVM vm);
        RefundOrderVM GetById(string recordId);
        IEnumerable<RefundOrderVM> GetAll();
    }
}
