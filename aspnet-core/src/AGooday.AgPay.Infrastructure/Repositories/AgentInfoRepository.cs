using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class AgentInfoRepository : AgPayRepository<AgentInfo>, IAgentInfoRepository
    {
        public AgentInfoRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<bool> IsExistAgentNoAsync(string agentNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.AgentNo.Equals(agentNo));
        }

        public Task<bool> IsExistAgentAsync(string isvNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.IsvNo.Equals(isvNo));
        }

        public async Task<IEnumerable<AgentInfo>> GetAllOrSubAgentsAsync(string currentAgentNo)
        {
            if (string.IsNullOrEmpty(currentAgentNo))
            {
                return GetAllAsNoTracking();
            }
            // 获取当前代理商的所有下级代理商，不包括下级的下级代理商
            var agent = await DbSet
                .Include(a => a.SubAgents)
                .Where(a => a.AgentNo == currentAgentNo)
                .FirstOrDefaultAsync();

            return agent?.SubAgents;
        }

        public async Task<IEnumerable<AgentInfo>> GetAllOrSubAgentsAsync(string currentAgentNo, Func<AgentInfo, bool> filter = null)
        {
            if (string.IsNullOrEmpty(currentAgentNo))
            {
                var query = GetAllAsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter).AsQueryable();
                }

                return query;
            }

            // 获取当前代理商的所有下级代理商，不包括下级的下级代理商
            var agent = await DbSet
                .Include(a => a.SubAgents)
                .Where(a => a.AgentNo == currentAgentNo)
                .FirstOrDefaultAsync();
            var subAgents = agent?.SubAgents;

            if (subAgents != null)
            {
                var query = subAgents.AsQueryable();

                if (filter != null)
                {
                    query = query.Where(filter).AsQueryable();
                }

                return query;
            }

            return Enumerable.Empty<AgentInfo>();
        }

        public async Task<ICollection<AgentInfo>> GetSubAgentsAsync(string agentNo)
        {
            // 获取当前代理商的所有下级代理商，不包括下级的下级代理商
            var agent = await DbSet
                .Include(a => a.SubAgents)
                .Where(a => a.AgentNo == agentNo)
                .FirstOrDefaultAsync();

            return agent?.SubAgents;
        }

        public async Task<IEnumerable<AgentInfo>> GetAllOrAllSubAgentsAsync(string currentAgentNo, Func<AgentInfo, bool> filter = null)
        {
            if (string.IsNullOrEmpty(currentAgentNo))
            {
                var query = GetAllAsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter).AsQueryable();
                }

                return query;
            }

            // 获取当前代理商的所有下级代理商，包括下级的下级代理商
            var subAgents = await DbSet
                .Include(a => a.SubAgents)
                .ThenInclude(subAgent => subAgent.SubAgents) // 包含下级代理商的下级代理商
                .Where(a => a.AgentNo == currentAgentNo)
                .SelectMany(a => a.SubAgents) // 使用 SelectMany 扁平化获取所有下级代理商
                .ToListAsync();

            if (subAgents != null)
            {
                var query = subAgents.AsQueryable();

                if (filter != null)
                {
                    query = query.Where(filter).AsQueryable();
                }

                return query;
            }

            return Enumerable.Empty<AgentInfo>();
        }

        public ICollection<AgentInfo> GetAllSubAgents(string agentNo)
        {
            // 获取当前代理商的所有下级代理商，包括下级的下级代理商
            var subAgents = DbSet
                .Include(a => a.SubAgents)
                .ThenInclude(subAgent => subAgent.SubAgents) // 包含下级代理商的下级代理商
                .Where(a => a.AgentNo == agentNo)
                .SelectMany(a => a.SubAgents) // 使用 SelectMany 扁平化获取所有下级代理商
                .ToList();

            return subAgents;
        }

        public IEnumerable<AgentInfo> GetParentAgents(string agentNo)
        {
            // 获取当前代理商的所有上级代理商，包括上级的上级代理商
            var currentAgent = DbSet
                .Include(a => a.ParentAgent)
                .FirstOrDefault(a => a.AgentNo == agentNo);

            var parentAgents = new List<AgentInfo>();

            while (currentAgent != null)
            {
                parentAgents.Add(currentAgent);
                if (currentAgent.ParentAgent != null)
                {
                    currentAgent = DbSet
                        .Include(a => a.ParentAgent)
                        .FirstOrDefault(a => a.AgentNo == currentAgent.ParentAgent.AgentNo);
                }
                else
                {
                    currentAgent = null;
                }
            }

            return parentAgents;
        }

        #region Sql
        public IEnumerable<AgentInfo> GetSubAgentsFromSqlAsNoTracking(string agentNo)
        {
            FormattableString sql = $@"WITH RECURSIVE sub_agents AS (
              SELECT * FROM t_agent_info WHERE agent_no = {agentNo}
              UNION ALL
              SELECT t.* FROM t_agent_info t INNER JOIN sub_agents sa ON t.pid = sa.agent_no
            )
            SELECT * FROM sub_agents;";
            return FromSqlAsNoTracking(sql);
        }

        public IEnumerable<AgentInfo> GetParentAgentsFromSqlAsNoTracking(string agentNo)
        {
            FormattableString sql = $@"WITH RECURSIVE parent_agents AS (
              SELECT * FROM t_agent_info WHERE agent_no = {agentNo}
              UNION ALL
              SELECT t.* FROM t_agent_info t INNER JOIN parent_agents pa ON t.agent_no = pa.pid
            )
            SELECT * FROM parent_agents;";
            return FromSqlAsNoTracking(sql);
        }
        #endregion

        #region 递归
        public List<AgentInfo> GetSubAgents(string currentAgentNo, Func<AgentInfo, bool> filter = null)
        {
            var query = DbSet.AsQueryable();

            if (!string.IsNullOrEmpty(currentAgentNo))
            {
                query = query.Where(a => a.AgentNo == currentAgentNo);
            }

            if (filter != null)
            {
                query = query.Where(filter).AsQueryable();
            }

            return GetSubAgentsRecursive(query.ToList());
        }

        private static List<AgentInfo> GetSubAgentsRecursive(List<AgentInfo> agents)
        {
            var subAgents = new List<AgentInfo>();

            foreach (var agent in agents)
            {
                subAgents.Add(agent);

                var childAgents = GetSubAgentsRecursive(agent.SubAgents.ToList());
                subAgents.AddRange(childAgents);
            }

            return subAgents;
        }

        public List<AgentInfo> GetParentAgents(string currentAgentNo, Func<AgentInfo, bool> filter = null)
        {
            var query = DbSet.AsQueryable();

            if (!string.IsNullOrEmpty(currentAgentNo))
            {
                query = query.Where(a => a.AgentNo == currentAgentNo);
            }

            if (filter != null)
            {
                query = query.Where(filter).AsQueryable();
            }

            return GetParentAgentsRecursive(query.ToList());
        }

        private static List<AgentInfo> GetParentAgentsRecursive(List<AgentInfo> agents)
        {
            var parentAgents = new List<AgentInfo>();

            foreach (var agent in agents)
            {
                parentAgents.Add(agent);

                if (agent.ParentAgent != null)
                {
                    var parent = GetParentAgentsRecursive(new List<AgentInfo> { agent.ParentAgent });
                    parentAgents.AddRange(parent);
                }
            }

            return parentAgents;
        }
        #endregion
    }
}
