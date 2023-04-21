using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class LevelRateConfigRepository : Repository<LevelRateConfig, long>, ILevelRateConfigRepository
    {
        public LevelRateConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public IQueryable<LevelRateConfig> GetByRateConfigId(long id)
        {
            return DbSet.Where(w => w.RateConfigId.Equals(id));
        }
    }
}
