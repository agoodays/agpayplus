using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAgentInfoService : IAgPayService<AgentInfoDto>
    {
        bool IsExistAgentNo(string mchNo);
        bool IsExistAgent(string isvNo);
        Task CreateAsync(AgentInfoCreateDto dto);
        Task RemoveAsync(string recordId);
        Task ModifyAsync(AgentInfoModifyDto dto);
        bool UpdateById(AgentInfoDto dto);
        IEnumerable<AgentInfoDto> GetParents(string agentNo);
        Task<PaginatedList<AgentInfoDto>> GetPaginatedDataAsync(AgentInfoQueryDto dto);
        Task<PaginatedList<AgentInfoDto>> GetPaginatedDataAsync(string agentNo, AgentInfoQueryDto dto);
    }
}
