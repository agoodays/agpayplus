using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchStoreRepository : AgPayRepository<MchStore>, IMchStoreRepository
    {
        public MchStoreRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public MchStore GetByIdAsNoTracking(long recordId)
        {
            return GetAllAsNoTracking().FirstOrDefault(w => w.StoreId.Equals(recordId));
        }
    }
}
