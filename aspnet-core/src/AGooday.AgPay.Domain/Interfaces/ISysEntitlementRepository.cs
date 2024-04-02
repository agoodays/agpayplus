using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysEntitlementRepository : IAgPayRepository<SysEntitlement>
    {
        SysEntitlement GetByKeyAsNoTracking(string entId, string sysType);
        SysEntitlement GetByKey(string entId, string sysType);
    }
}
