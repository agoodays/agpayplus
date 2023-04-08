using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayInterfaceConfigRepository : Repository<PayInterfaceConfig, long>, IPayInterfaceConfigRepository
    {
        public PayInterfaceConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistUseIfCode(string ifCode)
        {
            return DbSet.AsNoTracking().Any(c => c.IfCode.Equals(ifCode));
        }

        public bool MchAppHasAvailableIfCode(string appId, string ifCode)
        {
            return DbSet.AsNoTracking().Any(c => c.IfCode.Equals(ifCode)
            && c.InfoId.Equals(appId) && c.State.Equals(CS.PUB_USABLE) && c.InfoType.Equals(CS.INFO_TYPE_MCH_APP));
        }

        public void RemoveByInfoIds(List<string> infoIds, string infoType)
        {
            foreach (string infoId in infoIds)
            {
                var entity = DbSet.Where(w => w.InfoId.Equals(infoId) && w.InfoType.Equals(infoType)).First();
                Remove(entity.Id);
            }
        }
    }
}
