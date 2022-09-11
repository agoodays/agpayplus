using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
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
    }
}
