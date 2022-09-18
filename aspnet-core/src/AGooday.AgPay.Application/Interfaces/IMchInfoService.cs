using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchInfoService : IDisposable
    {
        void Add(MchInfoVM vm);
        void Remove(string recordId);
        void Update(MchInfoVM vm);
        MchInfoVM GetById(string recordId);
        IEnumerable<MchInfoVM> GetAll();
        PaginatedList<MchInfoVM> GetPaginatedData(MchInfoVM vm, int pageIndex, int pageSize);
    }
}
