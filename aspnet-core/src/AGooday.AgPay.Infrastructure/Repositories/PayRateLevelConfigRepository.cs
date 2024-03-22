using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayRateLevelConfigRepository : Repository<PayRateLevelConfig, long>, IPayRateLevelConfigRepository
    {
        public PayRateLevelConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public IQueryable<PayRateLevelConfig> GetByRateConfigId(long id)
        {
            return DbSet.Where(w => w.RateConfigId.Equals(id));
        }

        public IQueryable<PayRateLevelConfig> GetByRateConfigIds(List<long> ids)
        {
            return DbSet.Where(w => ids.Contains(w.RateConfigId));
        }
    }
}
