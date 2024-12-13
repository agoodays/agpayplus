using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserRoleRelaRepository : IAgPayRepository<SysUserRoleRela>
    {
        Task<bool> IsAssignedToUserAsync(string roleId);
        void RemoveByUserId(long userId);
    }
}
