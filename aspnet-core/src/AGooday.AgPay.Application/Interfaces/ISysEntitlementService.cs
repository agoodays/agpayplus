using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysEntitlementService : IAgPayService<SysEntitlementDto>
    {
        Task<SysEntitlementDto> GetByKeyAsNoTrackingAsync(string entId, string sysType);
        Task<SysEntitlementDto> GetByKeyAsync(string entId, string sysType);
        IEnumerable<SysEntitlementDto> GetBySysType(string sysType, string entId);
        IEnumerable<SysEntitlementDto> GetByIds(string sysType, List<string> entIds);
        IEnumerable<SysEntitlementDto> GetBros(string sysType, string pId);
        IEnumerable<SysEntitlementDto> GetBros(string sysType, string pId, string entId);
        IEnumerable<SysEntitlementDto> GetSubSysEntitlementsFromSql(string entId, string sysType);
        IEnumerable<SysEntitlementDto> GetParentSysEntitlementsFromSql(string entId, string sysType);
    }
}
