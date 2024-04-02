using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysConfigRepository : AgPayRepository<SysConfig>, ISysConfigRepository
    {
        public SysConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistSysConfig(string configKey)
        {
            return DbSet.AsNoTracking().Any(c => c.ConfigKey.Equals(configKey));
        }

        public SysConfig GetByKey(string configKey, string sysType, string belongInfoId)
        {
            return DbSet.FirstOrDefault(w => w.ConfigKey.Equals(configKey) && w.SysType.Equals(sysType) && w.BelongInfoId.Equals(belongInfoId));
        }
    }
}
