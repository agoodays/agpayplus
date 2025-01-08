using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayInterfaceConfigRepository : AgPayRepository<PayInterfaceConfig, long>, IPayInterfaceConfigRepository
    {
        public PayInterfaceConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<bool> IsExistUseIfCodeAsync(string ifCode)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.IfCode.Equals(ifCode));
        }

        public Task<bool> MchAppHasAvailableIfCodeAsync(string appId, string ifCode)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.IfCode.Equals(ifCode)
            && c.InfoId.Equals(appId) && c.State.Equals(CS.PUB_USABLE) && c.InfoType.Equals(CS.INFO_TYPE.MCH_APP));
        }

        public void RemoveByInfoIds(IQueryable<string> infoIds, string infoType)
        {
            var entities = DbSet.Where(w => infoIds.Contains(w.InfoId) && w.InfoType.Equals(infoType));
            RemoveRange(entities);
        }
    }
}
