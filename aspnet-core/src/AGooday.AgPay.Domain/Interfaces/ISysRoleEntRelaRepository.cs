using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysRoleEntRelaRepository : IRepository<SysRoleEntRela>
    {
        void RemoveByRoleId(string roleId);
    }
}
