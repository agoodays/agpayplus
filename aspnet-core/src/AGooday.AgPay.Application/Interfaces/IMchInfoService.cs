using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchInfoService : IDisposable
    {
        void Add(MchInfoDto dto);
        void Remove(string recordId);
        void Update(MchInfoDto dto);
        MchInfoDto GetById(string recordId);
        IEnumerable<MchInfoDto> GetAll();
        PaginatedList<MchInfoDto> GetPaginatedData(MchInfoDto dto, int pageIndex, int pageSize);
    }
}
