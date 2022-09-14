using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ITransferOrderService : IDisposable
    {
        void Add(TransferOrderVM vm);
        void Remove(string recordId);
        void Update(TransferOrderVM recordId);
        TransferOrderVM GetById(string recordId);
        IEnumerable<TransferOrderVM> GetAll();
    }
}
