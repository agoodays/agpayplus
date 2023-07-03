using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchInfoService : IDisposable
    {
        bool IsExistMchNo(string mchNo);
        bool IsExistMchByIsvNo(string isvNo);
        bool IsExistMchByAgentNo(string agentNo);
        bool Add(MchInfoDto dto);
        void Create(MchInfoCreateDto dto);
        void Remove(string recordId);
        bool Update(MchInfoDto dto);
        bool UpdateById(MchInfoUpdateDto dto);
        void Modify(MchInfoModifyDto dto);
        MchInfoDto GetById(string recordId);
        MchInfoDetailDto GetByMchNo(string mchNo);
        IEnumerable<MchInfoDto> GetByMchNos(List<string> mchNos);
        IEnumerable<MchInfoDto> GetAll();
        PaginatedList<MchInfoDto> GetPaginatedData(MchInfoQueryDto dto);
    }
}
