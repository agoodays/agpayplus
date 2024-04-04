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

        public MchApp GetAsNoTrackingById(string id)
        {
            return GetAllAsNoTracking().FirstOrDefault(w => w.AppId.Equals(id));
        }
    }
}
