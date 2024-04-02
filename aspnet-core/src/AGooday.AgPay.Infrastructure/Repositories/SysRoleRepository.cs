using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysRoleRepository : AgPayRepository<SysRole>, ISysRoleRepository
    {
        public SysRoleRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
