using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchInfoService : IAgPayService<MchInfoDto>
    {
        bool IsExistMchNo(string mchNo);
        bool IsExistMchByIsvNo(string isvNo);
        bool IsExistMchByAgentNo(string agentNo);
        Task CreateAsync(MchInfoCreateDto dto);
        Task RemoveAsync(string recordId);
        Task ModifyAsync(MchInfoModifyDto dto);
        bool UpdateById(MchInfoDto dto);
        IEnumerable<MchInfoDto> GetByMchNos(List<string> mchNos);
        IEnumerable<MchInfoDto> GetByIsvNo(string isvNo);
        Task<PaginatedList<MchInfoDto>> GetPaginatedDataAsync(MchInfoQueryDto dto);
    }
}
