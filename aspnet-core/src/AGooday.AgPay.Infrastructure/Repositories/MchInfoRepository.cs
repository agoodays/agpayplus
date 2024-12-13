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

        public Task<bool> IsExistMchNoAsync(string mchNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.MchNo.Equals(mchNo));
        }

        public Task<bool> IsExistMchByIsvNoAsync(string isvNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.IsvNo.Equals(isvNo) && c.Type.Equals(CS.MCH_TYPE_ISVSUB));
        }

        public Task<bool> IsExistMchByAgentNoAsync(string agentNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.AgentNo.Equals(agentNo) && c.Type.Equals(CS.MCH_TYPE_ISVSUB));
        }
    }
}
