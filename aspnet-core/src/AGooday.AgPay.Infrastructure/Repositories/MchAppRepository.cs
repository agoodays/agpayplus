using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchAppRepository : Repository<MchApp>, IMchAppRepository
    {
        public MchAppRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
