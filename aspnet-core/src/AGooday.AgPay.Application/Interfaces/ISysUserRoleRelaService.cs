using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserRoleRelaService : IAgPayService<SysUserRoleRelaDto>
    {
        void SaveUserRole(long userId, List<string> roleIds);
        IEnumerable<string> SelectRoleIdsByUserId(long userId);
        IEnumerable<long> SelectUserIdsByRoleId(string roleId);
        PaginatedList<SysUserRoleRelaDto> GetPaginatedData(SysUserRoleRelaQueryDto dto);
    }
}
