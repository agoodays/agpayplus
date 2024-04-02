using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysRoleEntRelaRepository : IAgPayRepository<SysRoleEntRela>
    {
        void RemoveByRoleId(string roleId);
    }
}
