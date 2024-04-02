using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysUserTeamRepository : AgPayRepository<SysUserTeam>, ISysUserTeamRepository
    {
        public SysUserTeamRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
