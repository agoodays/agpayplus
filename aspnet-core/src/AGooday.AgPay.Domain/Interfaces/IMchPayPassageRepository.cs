using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchPayPassageRepository : IAgPayRepository<MchPayPassage, long>
    {
        IQueryable<MchPayPassage> GetMchPayPassageByMchNoAndAppId(string mchNo, string appId);
        IQueryable<MchPayPassage> GetByAppIdAndWayCodesAsNoTracking(string appId, List<string> wayCodes);
        bool IsExistMchPayPassageUseWayCode(string wayCode);
        void RemoveByMchNo(string mchNo);
    }
}
