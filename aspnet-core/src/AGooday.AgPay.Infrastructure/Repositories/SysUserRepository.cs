using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysUserRepository : Repository<SysUser, long>, ISysUserRepository
    {
        public SysUserRepository(AgPayDbContext context) 
            : base(context)
        {
        }

        public SysUser GetByLoginUsername(string loginUsername)
        {
            return DbSet.AsNoTracking().FirstOrDefault(c => c.LoginUsername == loginUsername);
        }
    }
}
