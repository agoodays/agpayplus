using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysEntitlementService : IAgPayService<SysEntitlementDto>
    {
        SysEntitlementDto GetByKeyAsNoTracking(string entId, string sysType);
        SysEntitlementDto GetByKey(string entId, string sysType);
        IEnumerable<SysEntitlementDto> GetBySysType(string sysType, string entId);
        IEnumerable<SysEntitlementDto> GetByIds(string sysType, List<string> entIds);
        IEnumerable<SysEntitlementDto> GetSons(string sysType, string pId, string entId);
    }
}
