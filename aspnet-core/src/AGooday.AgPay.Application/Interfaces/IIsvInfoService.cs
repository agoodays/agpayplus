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
        bool Add(IsvInfoDto dto);
        bool Remove(string recordId);
        bool Update(IsvInfoDto dto);
        IsvInfoDto GetById(string recordId);
        IEnumerable<IsvInfoDto> GetAll();
        PaginatedList<IsvInfoDto> GetPaginatedData(IsvInfoQueryDto dto);
        bool IsExistIsvNo(string isvNo);
    }
}
