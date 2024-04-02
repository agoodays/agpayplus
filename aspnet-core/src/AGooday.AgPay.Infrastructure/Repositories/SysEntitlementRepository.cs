using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysEntitlementRepository : AgPayRepository<SysEntitlement>, ISysEntitlementRepository
    {
        public SysEntitlementRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public SysEntitlement GetByKeyAsNoTracking(string entId, string sysType)
        {
            return DbSet.AsNoTracking().FirstOrDefault(w => w.SysType.Equals(sysType) && w.EntId.Equals(entId));
        }

        public SysEntitlement GetByKey(string entId, string sysType)
        {
            return DbSet.FirstOrDefault(w => w.SysType.Equals(sysType) && w.EntId.Equals(entId));
        }
    }
}
