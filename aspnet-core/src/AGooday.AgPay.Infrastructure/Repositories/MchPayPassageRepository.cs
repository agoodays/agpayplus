using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchPayPassageRepository : AgPayRepository<MchPayPassage, long>, IMchPayPassageRepository
    {
        public MchPayPassageRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public IQueryable<MchPayPassage> GetMchPayPassageByMchNoAndAppId(string mchNo, string appId)
        {
            return GetAllAsNoTracking().Where(w => w.MchNo.Equals(mchNo) && w.AppId.Equals(appId) && w.State.Equals(CS.PUB_USABLE));
        }

        public IQueryable<MchPayPassage> GetByAppIdAndWayCodesAsNoTracking(string appId, List<string> wayCodes)
        {
            return GetAllAsNoTracking().Where(w => w.AppId.Equals(appId) && (wayCodes.Count == 0 || wayCodes.Contains(w.WayCode)));
        }

        public bool IsExistMchPayPassageUseWayCode(string wayCode)
        {
            return GetAllAsNoTracking().Any(c => c.WayCode.Equals(wayCode));
        }

        public void RemoveByMchNo(string mchNo)
        {
            var mchPayPassages = DbSet.Where(w => w.MchNo.Equals(mchNo));
            foreach (var mchPayPassage in mchPayPassages)
            {
                Remove(mchPayPassage.Id);
            }
        }
    }
}
