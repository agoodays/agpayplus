using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchInfoService : IDisposable
    {
        bool IsExistMchNo(string mchNo);
        bool Add(MchInfoDto dto);
        void Create(MchInfoCreateDto dto);
        void Remove(string recordId);
        bool Update(MchInfoDto dto);
        void Modify(MchInfoModifyDto dto);
        MchInfoDto GetById(string recordId);
        MchInfoDetailDto GetByMchNo(string mchNo);
        IEnumerable<MchInfoDto> GetAll();
        PaginatedList<MchInfoDto> GetPaginatedData(MchInfoQueryDto dto);
    }
}
