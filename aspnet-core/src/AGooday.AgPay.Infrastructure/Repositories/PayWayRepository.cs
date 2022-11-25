using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayWayRepository : Repository<PayWay>, IPayWayRepository
    {
        public PayWayRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistPayWayCode(string wayCode)
        {
            return DbSet.AsNoTracking().Any(c => c.WayCode == wayCode.ToUpper());
        }
    }
}
