using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayWayRepository : AgPayRepository<PayWay>, IPayWayRepository
    {
        public PayWayRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistPayWayCode(string wayCode)
        {
            return GetAllAsNoTracking().Any(c => c.WayCode == wayCode.ToUpper());
        }
    }
}
