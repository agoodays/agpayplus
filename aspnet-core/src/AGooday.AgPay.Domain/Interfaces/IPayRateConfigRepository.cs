using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayRateConfigRepository : IRepository<PayRateConfig, long>
    {
        PayRateConfig GetByUniqueKey(string configType, string infoType, string infoId, string ifCode, string wayCode);
        IQueryable<PayRateConfig> GetByInfoIdAndIfCode(string configType, string infoType, string infoId, string ifCode);
        IQueryable<PayRateConfig> GetByInfoIdAndIfCodeAsNoTracking(string configType, string infoType, string infoId, string ifCode);
        IQueryable<PayRateConfig> GetByInfoId(string configType, string infoType, string infoId);
        IQueryable<PayRateConfig> GetByInfoIdAsNoTracking(string configType, string infoType, string infoId);
    }
}
