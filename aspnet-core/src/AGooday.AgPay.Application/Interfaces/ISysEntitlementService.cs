using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysEntitlementService : IDisposable
    {
        void Add(SysEntitlementDto dto);
        void Remove(string recordId);
        void Update(SysEntitlementDto dto);
        SysEntitlementDto GetById(string recordId);
        SysEntitlementDto GetByKeyAsNoTracking(string entId, string sysType);
        SysEntitlementDto GetByKey(string entId, string sysType);
        IEnumerable<SysEntitlementDto> GetBySysType(string sysType, string entId);
        IEnumerable<SysEntitlementDto> GetByIds(string sysType, List<string> entIds);
        IEnumerable<SysEntitlementDto> GetSons(string sysType, string pId, string entId);
        IEnumerable<SysEntitlementDto> GetAll();
    }
}
