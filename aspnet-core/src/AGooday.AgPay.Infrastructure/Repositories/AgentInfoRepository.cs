using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class AgentInfoRepository : Repository<AgentInfo>, IAgentInfoRepository
    {
        public AgentInfoRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistAgentNo(string agentNo)
        {
            return DbSet.AsNoTracking().Any(c => c.AgentNo.Equals(agentNo));
        }
    }
}
