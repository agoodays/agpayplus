using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleEntRelaService : IAgPayService<SysRoleEntRelaDto>
    {
        Task<PaginatedList<SysRoleEntRelaDto>> GetPaginatedDataAsync(SysRoleEntRelaQueryDto dto);
        IEnumerable<string> SelectEntIdsByUserId(long userId, byte userType, string sysType);
        IEnumerable<SysEntitlementDto> SelectEntsByUserId(long userId, byte userType, string sysType);
        Task<int> ResetRelaAsync(string roleId, List<string> entIdList);
    }
}
