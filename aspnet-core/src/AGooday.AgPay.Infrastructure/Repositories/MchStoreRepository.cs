using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchStoreRepository : Repository<MchStore>, IMchStoreRepository
    {
        public MchStoreRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
