using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAgentInfoService : IAgPayService<AgentInfoDto>
    {
        Task<bool> IsExistAgentNoAsync(string mchNo);
        Task<bool> IsExistAgentAsync(string isvNo);
        Task CreateAsync(AgentInfoCreateDto dto);
        Task RemoveAsync(string recordId);
        Task ModifyAsync(AgentInfoModifyDto dto);
        Task<bool> UpdateByIdAsync(AgentInfoDto dto);
        IEnumerable<AgentInfoDto> GetParents(string agentNo);
        Task<PaginatedResult<AgentInfoDto>> GetPaginatedDataAsync(AgentInfoQueryDto dto);
        Task<PaginatedResult<AgentInfoDto>> GetPaginatedDataAsync(string agentNo, AgentInfoQueryDto dto);
    }
}
