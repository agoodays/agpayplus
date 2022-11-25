using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysEntitlementService : IDisposable
    {
        void Add(SysEntitlementDto dto);
        void Remove(string recordId);
        void Update(SysEntModifyDto dto);
        SysEntitlementDto GetById(string recordId);
        IEnumerable<SysEntitlementDto> GetBySysType(string sysType, string entId);
        IEnumerable<SysEntitlementDto> GetBySysType(string sysType, List<string> entIds, List<string> entTypes);
        IEnumerable<SysEntitlementDto> GetAll();
    }
}
