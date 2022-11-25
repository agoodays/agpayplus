using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysEntitlementRepository : Repository<SysEntitlement>, ISysEntitlementRepository
    {
        public SysEntitlementRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
