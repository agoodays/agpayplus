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
