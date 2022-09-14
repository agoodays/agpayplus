using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverService : IDisposable
    {
        void Add(MchDivisionReceiverVM vm);
        void Remove(long recordId);
        void Update(MchDivisionReceiverVM recordId);
        MchDivisionReceiverVM GetById(long recordId);
        IEnumerable<MchDivisionReceiverVM> GetAll();
    }
}
