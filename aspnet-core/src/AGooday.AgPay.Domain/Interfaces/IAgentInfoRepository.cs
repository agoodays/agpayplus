using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IAgentInfoRepository : IRepository<AgentInfo>
    {
        bool IsExistAgentNo(string agentNo);
        bool IsExistAgent(string isvNo);
    }
}
