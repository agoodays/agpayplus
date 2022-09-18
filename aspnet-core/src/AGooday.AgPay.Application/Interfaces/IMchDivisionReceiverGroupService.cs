using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverGroupService : IDisposable
    {
        void Add(MchDivisionReceiverGroupVM vm);
        void Remove(long recordId);
        void Update(MchDivisionReceiverGroupVM vm);
        MchDivisionReceiverGroupVM GetById(long recordId);
        IEnumerable<MchDivisionReceiverGroupVM> GetAll();
    }
}
