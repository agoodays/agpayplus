using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserRoleRelaRepository : IAgPayRepository<SysUserRoleRela>
    {
        bool IsAssignedToUser(string roleId);
        void RemoveByUserId(long userId);
    }
}
