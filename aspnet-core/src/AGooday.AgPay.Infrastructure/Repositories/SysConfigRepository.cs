using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGooday.AgPay.Common.Constants.CS;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysConfigRepository : Repository<SysConfig>, ISysConfigRepository
    {
        public SysConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistSysConfig(string configKey)
        {
            return DbSet.AsNoTracking().Any(c => c.ConfigKey == configKey);
        }
    }
}
