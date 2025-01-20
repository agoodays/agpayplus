using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysEntitlementRepository : IAgPayRepository<SysEntitlement>
    {
        Task<SysEntitlement> GetByKeyAsNoTrackingAsync(string entId, string sysType);
        Task<SysEntitlement> GetByKeyAsync(string entId, string sysType);
        IEnumerable<SysEntitlement> GetSubSysEntitlementsFromSqlAsNoTracking(string entId, string sysType);
        IEnumerable<SysEntitlement> GetParentSysEntitlementsFromSqlAsNoTracking(string entId, string sysType);
    }
}
