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
        bool IsExistMchNo(string mchNo);
        void Add(MchInfoDto dto);
        void Create(MchInfoCreateDto dto);
        void Remove(string recordId);
        void Update(MchInfoDto dto);
        void Modify(MchInfoModifyDto dto);
        MchInfoDto GetById(string recordId);
        MchInfoDetailDto GetByMchNo(string mchNo);
        IEnumerable<MchInfoDto> GetAll();
        PaginatedList<MchInfoDto> GetPaginatedData(MchInfoQueryDto dto);
    }
}
