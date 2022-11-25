using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserRoleRelaRepository : IRepository<SysUserRoleRela>
    {
        bool IsAssignedToUser(string roleId);
        void RemoveByUserId(long userId);
    }
}
