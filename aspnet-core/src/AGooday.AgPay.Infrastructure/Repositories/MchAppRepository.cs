using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchAppRepository : AgPayRepository<MchApp>, IMchAppRepository
    {
        public MchAppRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public MchApp GetByIdAsNoTracking(string recordId)
        {
            return GetAllAsNoTracking().FirstOrDefault(w => w.AppId.Equals(recordId));
        }

        public MchApp GetById(string recordId, string mchNo)
        {
            return GetAll().FirstOrDefault(w => w.MchNo.Equals(mchNo) && w.AppId.Equals(recordId));
        }

        public MchApp GetByIdAsNoTracking(string recordId, string mchNo)
        {
            return GetAllAsNoTracking().FirstOrDefault(w => w.MchNo.Equals(mchNo) && w.AppId.Equals(recordId));
        }
    }
}
