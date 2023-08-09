using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysEntitlementRepository : Repository<SysEntitlement>, ISysEntitlementRepository
    {
        public SysEntitlementRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public SysEntitlement GetByKeyAsNoTracking(string entId, string sysType)
        {
            var entity = DbSet.AsNoTracking()
                .Where(w => w.SysType.Equals(sysType) && w.EntId.Equals(entId))
                .FirstOrDefault();

            return entity;
        }

        public SysEntitlement GetByKey(string entId, string sysType)
        {
            var entity = DbSet
                .Where(w => w.SysType.Equals(sysType) && w.EntId.Equals(entId))
                .FirstOrDefault();

            return entity;
        }
    }
}
