using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayRateLevelConfigRepository : IAgPayRepository<PayRateLevelConfig, long>
    {
        IQueryable<PayRateLevelConfig> GetByRateConfigId(long id);
        IQueryable<PayRateLevelConfig> GetByRateConfigIds(List<long> ids);
    }
}
