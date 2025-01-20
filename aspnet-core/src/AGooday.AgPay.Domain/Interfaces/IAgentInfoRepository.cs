using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IAgentInfoRepository : IAgPayRepository<AgentInfo>
    {
        Task<bool> IsExistAgentNoAsync(string agentNo);
        Task<bool> IsExistAgentAsync(string isvNo);
        Task<IEnumerable<AgentInfo>> GetAllOrSubAgentsAsync(string currentAgentNo);
        Task<IEnumerable<AgentInfo>> GetAllOrSubAgentsAsync(string currentAgentNo, Func<AgentInfo, bool> filter = null);
        Task<ICollection<AgentInfo>> GetSubAgentsAsync(string agentNo);
        Task<IEnumerable<AgentInfo>> GetAllOrAllSubAgentsAsync(string currentAgentNo, Func<AgentInfo, bool> filter = null);
        ICollection<AgentInfo> GetAllSubAgents(string agentNo);
        IEnumerable<AgentInfo> GetParentAgents(string agentNo);
        IEnumerable<AgentInfo> GetSubAgentsFromSqlAsNoTracking(string agentNo);
        IEnumerable<AgentInfo> GetParentAgentsFromSqlAsNoTracking(string agentNo);
    }
}
