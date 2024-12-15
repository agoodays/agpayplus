using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchInfoService : IAgPayService<MchInfoDto>
    {
        Task<bool> IsExistMchNoAsync(string mchNo);
        Task<bool> IsExistMchByIsvNoAsync(string isvNo);
        Task<bool> IsExistMchByAgentNoAsync(string agentNo);
        Task CreateAsync(MchInfoCreateDto dto);
        Task RemoveAsync(string recordId);
        Task ModifyAsync(MchInfoModifyDto dto);
        Task<bool> UpdateByIdAsync(MchInfoDto dto);
        IEnumerable<MchInfoDto> GetByMchNos(List<string> mchNos);
        IEnumerable<MchInfoDto> GetByIsvNo(string isvNo);
        Task<PaginatedList<MchInfoDto>> GetPaginatedDataAsync(MchInfoQueryDto dto);
    }
}
