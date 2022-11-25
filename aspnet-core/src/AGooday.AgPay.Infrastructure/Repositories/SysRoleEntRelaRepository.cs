using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysRoleEntRelaRepository : Repository<SysRoleEntRela>, ISysRoleEntRelaRepository
    {
        public SysRoleEntRelaRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public void RemoveByRoleId(string roleId)
        {
            var entitys = DbSet.Where(w=>w.RoleId == roleId);
            foreach (var entity in entitys)
            {
                DbSet.Remove(entity);
            }
        }
    }
}
