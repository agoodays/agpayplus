using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAgentInfoService : IDisposable
    {
        bool IsExistAgentNo(string mchNo);
        bool IsExistAgent(string isvNo);
        bool Add(AgentInfoDto dto);
        void Create(AgentInfoCreateDto dto);
        void Remove(string recordId);
        bool Update(AgentInfoDto dto);
        bool UpdateById(AgentInfoUpdateDto dto);
        void Modify(AgentInfoModifyDto dto);
        AgentInfoDto GetById(string recordId);
        AgentInfoDetailDto GetByAgentNo(string agentNo);
        IEnumerable<AgentInfoDto> GetAll();
        IEnumerable<AgentInfoDto> GetParents(string agentNo);
        PaginatedList<AgentInfoDto> GetPaginatedData(AgentInfoQueryDto dto);
        PaginatedList<AgentInfoDto> GetPaginatedData(string agentNo, AgentInfoQueryDto dto);
    }
}
