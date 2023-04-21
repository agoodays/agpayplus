using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayRateConfigRepository : Repository<PayRateConfig, long>, IPayRateConfigRepository
    {
        public PayRateConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public PayRateConfig GetByUniqueKey(string configType, string infoType, string infoId, string ifCode, string wayCode)
        {
            return DbSet.Where(w => w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && w.InfoId.Equals(infoId)
            && w.IfCode.Equals(ifCode)
            && w.WayCode.Equals(wayCode)).FirstOrDefault();
        }

        public IQueryable<PayRateConfig> GetByInfoIdAndIfCode(string configType, string infoType, string infoId, string ifCode)
        {
            return DbSet.Where(w => w.ConfigType.Equals(configType)
            && w.InfoType.Equals(infoType)
            && w.InfoId.Equals(infoId)
            && w.IfCode.Equals(ifCode));
        }
    }
}
