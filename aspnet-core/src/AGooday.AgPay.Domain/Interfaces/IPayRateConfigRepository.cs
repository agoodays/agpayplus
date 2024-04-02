using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayRateConfigRepository : IAgPayRepository<PayRateConfig, long>
    {
        PayRateConfig GetByUniqueKey(string configType, string infoType, string infoId, string ifCode, string wayCode);
        IQueryable<PayRateConfig> GetByUniqueKeysAsNoTracking(string configType, string infoType, string ifCode, string wayCode, List<string> infoIds);
        IQueryable<PayRateConfig> GetByInfoIdAndIfCode(string configType, string infoType, string infoId, string ifCode);
        IQueryable<PayRateConfig> GetByInfoIdAndIfCodeAsNoTracking(string configType, string infoType, string infoId, string ifCode);
        IQueryable<PayRateConfig> GetByInfoId(string configType, string infoType, string infoId);
        IQueryable<PayRateConfig> GetByInfoIdAsNoTracking(string configType, string infoType, string infoId);
    }
}
