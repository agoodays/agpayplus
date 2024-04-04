using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleEntRelaService : IAgPayService<SysRoleEntRelaDto>
    {
        PaginatedList<SysRoleEntRelaDto> GetPaginatedData(SysRoleEntRelaQueryDto dto);
        bool UserHasLeftMenu(long userId, string sysType);
        IEnumerable<string> SelectEntIdsByUserId(long userId, byte userType, string sysType);
        IEnumerable<SysEntitlementDto> SelectEntsByUserId(long userId, byte userType, string sysType);
        void ResetRela(string roleId, List<string> entIdList);
    }
}
