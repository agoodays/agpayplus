using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchInfoService : IDisposable
    {
        bool IsExistMchNo(string mchNo);
        bool IsExistMchByIsvNo(string isvNo);
        bool IsExistMchByAgentNo(string agentNo);
        bool Add(MchInfoDto dto);
        Task CreateAsync(MchInfoCreateDto dto);
        Task RemoveAsync(string recordId);
        bool Update(MchInfoDto dto);
        bool UpdateById(MchInfoUpdateDto dto);
        Task ModifyAsync(MchInfoModifyDto dto);
        MchInfoDto GetById(string recordId);
        Task<MchInfoDto> GetByIdAsync(string recordId);
        IEnumerable<MchInfoDto> GetByMchNos(List<string> mchNos);
        IEnumerable<MchInfoDto> GetAll();
        PaginatedList<MchInfoDto> GetPaginatedData(MchInfoQueryDto dto);
        IEnumerable<MchInfoDto> GetByIsvNo(string isvNo);
    }
}
