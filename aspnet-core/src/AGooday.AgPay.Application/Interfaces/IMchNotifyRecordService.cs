using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchNotifyRecordService : IDisposable
    {
        void Add(MchNotifyRecordVM vm);
        void Remove(long recordId);
        void Update(MchNotifyRecordVM recordId);
        MchNotifyRecordVM GetById(long recordId);
        IEnumerable<MchNotifyRecordVM> GetAll();
    }
}
