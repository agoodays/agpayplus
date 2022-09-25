using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchAppService : IDisposable
    {
        void Add(MchAppDto dto);
        void Remove(string recordId);
        void Update(MchAppDto dto);
        MchAppDto GetById(string recordId);
        IEnumerable<MchAppDto> GetAll();
        PaginatedList<MchAppDto> GetPaginatedData(MchAppQueryDto dto);
    }
}
