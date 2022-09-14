using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchPayPassageService : IDisposable
    {
        void Add(MchPayPassageVM vm);
        void Remove(long recordId);
        void Update(MchPayPassageVM recordId);
        MchPayPassageVM GetById(long recordId);
        IEnumerable<MchPayPassageVM> GetAll();
    }
}
