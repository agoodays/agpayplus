using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchStoreRepository : Repository<MchStore>, IMchStoreRepository
    {
        public MchStoreRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public MchStore GetByKeyAsNoTracking(long recordId)
        {
            return DbSet.AsNoTracking().FirstOrDefault(w => w.StoreId.Equals(recordId));
        }
    }
}
