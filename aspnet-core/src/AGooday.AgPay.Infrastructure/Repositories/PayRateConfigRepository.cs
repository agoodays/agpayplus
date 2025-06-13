using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayRateConfigRepository : AgPayRepository<PayRateConfig, long>, IPayRateConfigRepository
    {
        public PayRateConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public PayRateConfig GetByUniqueKey(string configType, string infoType, string infoId, string ifCode, string wayCode)
        {
            return DbSet.FirstOrDefault(w => w.State.Equals(CS.YES)
            && w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && w.InfoId.Equals(infoId)
            && w.IfCode.Equals(ifCode)
            && w.WayCode.Equals(wayCode));
        }

        public async Task<PayRateConfig> GetByUniqueKeyAsNoTrackingAsync(string configType, string infoType, string infoId, string ifCode, string wayCode)
        {
            return await GetAllAsNoTracking().FirstOrDefaultAsync(w => w.State.Equals(CS.YES)
            && w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && w.InfoId.Equals(infoId)
            && w.IfCode.Equals(ifCode)
            && w.WayCode.Equals(wayCode));
        }

        public IQueryable<PayRateConfig> GetByUniqueKeysAsNoTracking(string configType, string infoType, string ifCode, string wayCode, List<string> infoIds)
        {
            return GetAllAsNoTracking().Where(w => w.State.Equals(CS.YES)
            && w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && infoIds.Contains(w.InfoId)
            && w.IfCode.Equals(ifCode)
            && w.WayCode.Equals(wayCode));
        }

        public IQueryable<PayRateConfig> GetByInfoIdAndIfCode(string configType, string infoType, string infoId, string ifCode)
        {
            return DbSet.Where(w => w.State.Equals(CS.YES)
            && w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && w.InfoId.Equals(infoId)
            && w.IfCode.Equals(ifCode));
        }

        public IQueryable<PayRateConfig> GetByInfoIdAndIfCodeAsNoTracking(string configType, string infoType, string infoId, string ifCode)
        {
            return GetAllAsNoTracking().Where(w => w.State.Equals(CS.YES)
            && w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && w.InfoId.Equals(infoId)
            && w.IfCode.Equals(ifCode));
        }

        public IQueryable<PayRateConfig> GetByInfoId(string configType, string infoType, string infoId)
        {
            return DbSet.Where(w => w.State.Equals(CS.YES)
            && w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && w.InfoId.Equals(infoId));
        }

        public IQueryable<PayRateConfig> GetByInfoIdAsNoTracking(string configType, string infoType, string infoId)
        {
            return GetAllAsNoTracking().Where(w => w.State.Equals(CS.YES)
            && w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && w.InfoId.Equals(infoId));
        }
    }
}
