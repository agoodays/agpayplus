using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayWayRepository : AgPayRepository<PayWay>, IPayWayRepository
    {
        public PayWayRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<bool> IsExistPayWayCodeAsync(string wayCode)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.WayCode == wayCode.ToUpper());
            //return GetAllAsNoTracking().AnyAsync(c => string.Equals(c.WayCode, wayCode, StringComparison.OrdinalIgnoreCase));
        }
    }
}
