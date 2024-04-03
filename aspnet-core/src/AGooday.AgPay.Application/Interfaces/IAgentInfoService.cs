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
        PaginatedList<AgentInfoDto> GetPaginatedData(AgentInfoQueryDto dto);
        PaginatedList<AgentInfoDto> GetPaginatedData(string agentNo, AgentInfoQueryDto dto);
    }
}
