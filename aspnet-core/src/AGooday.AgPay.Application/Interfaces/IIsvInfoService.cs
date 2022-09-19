using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IIsvInfoService : IDisposable
    {
        void Add(IsvInfoDto dto);
        void Remove(string recordId);
        void Update(IsvInfoDto dto);
        IsvInfoDto GetById(string recordId);
        IEnumerable<IsvInfoDto> GetAll();
        PaginatedList<IsvInfoDto> GetPaginatedData(IsvInfoDto dto, int pageIndex, int pageSize);
    }
}
