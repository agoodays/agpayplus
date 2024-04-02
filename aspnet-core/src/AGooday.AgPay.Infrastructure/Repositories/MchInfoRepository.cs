using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchInfoRepository : AgPayRepository<MchInfo>, IMchInfoRepository
    {
        public MchInfoRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistMchNo(string mchNo)
        {
            return DbSet.AsNoTracking().Any(c => c.MchNo.Equals(mchNo));
        }

        public bool IsExistMchByIsvNo(string isvNo)
        {
            return DbSet.AsNoTracking().Any(c => c.IsvNo.Equals(isvNo) && c.Type.Equals(CS.MCH_TYPE_ISVSUB));
        }

        public bool IsExistMchByAgentNo(string agentNo)
        {
            return DbSet.AsNoTracking().Any(c => c.AgentNo.Equals(agentNo) && c.Type.Equals(CS.MCH_TYPE_ISVSUB));
        }
    }
}
