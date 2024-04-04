using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

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
            return GetAllAsNoTracking().Any(c => c.ConfigKey.Equals(configKey));
        }

        public SysConfig GetByKey(string configKey, string sysType, string belongInfoId)
        {
            return DbSet.FirstOrDefault(w => w.ConfigKey.Equals(configKey) && w.SysType.Equals(sysType) && w.BelongInfoId.Equals(belongInfoId));
        }
    }
}
