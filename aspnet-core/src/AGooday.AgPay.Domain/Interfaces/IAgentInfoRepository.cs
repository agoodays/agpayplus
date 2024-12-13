using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IAgentInfoRepository : IAgPayRepository<AgentInfo>
    {
        Task<bool> IsExistAgentNoAsync(string agentNo);
        Task<bool> IsExistAgentAsync(string isvNo);
        IEnumerable<AgentInfo> GetAllOrSubAgents(string agentNo);
        IEnumerable<AgentInfo> GetAllOrSubAgents(string currentAgentNo, Func<AgentInfo, bool> filter = null);
        ICollection<AgentInfo> GetSubAgents(string agentNo);
        IEnumerable<AgentInfo> GetAllOrAllSubAgents(string currentAgentNo, Func<AgentInfo, bool> filter = null);
        ICollection<AgentInfo> GetAllSubAgents(string agentNo);
        IEnumerable<AgentInfo> GetParentAgents(string agentNo);
        IEnumerable<AgentInfo> GetSubAgentsFromSql(string agentNo);
        IEnumerable<AgentInfo> GetParentAgentsFromSql(string agentNo);
    }
}
