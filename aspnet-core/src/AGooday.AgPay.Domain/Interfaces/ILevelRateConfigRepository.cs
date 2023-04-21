using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ILevelRateConfigRepository : IRepository<LevelRateConfig, long>
    {
        IQueryable<LevelRateConfig> GetByRateConfigId(long id);
    }
}
