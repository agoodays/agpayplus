using AGooday.AgPay.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IIsvInfoService : IDisposable
    {
        void Add(IsvInfoVM vm);
        void Remove(string recordId);
        void Update(IsvInfoVM vm);
        IsvInfoVM GetById(string recordId);
        IEnumerable<IsvInfoVM> GetAll();
        PaginatedList<IsvInfoVM> GetPaginatedData(IsvInfoVM vm, int pageIndex, int pageSize);
    }
}
