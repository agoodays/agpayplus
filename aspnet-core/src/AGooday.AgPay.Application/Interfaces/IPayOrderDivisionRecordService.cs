using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderDivisionRecordService : IDisposable
    {
        void Add(PayOrderDivisionRecordVM vm);
        void Remove(long recordId);
        void Update(PayOrderDivisionRecordVM vm);
        PayOrderDivisionRecordVM GetById(long recordId);
        IEnumerable<PayOrderDivisionRecordVM> GetAll();
    }
}
