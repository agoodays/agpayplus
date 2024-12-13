using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayOrderRepository : AgPayRepository<PayOrder>, IPayOrderRepository
    {
        public PayOrderRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<bool> IsExistOrderUseIfCodeAsync(string ifCode)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.IfCode.Equals(ifCode));
        }

        public Task<bool> IsExistOrderUseMchNoAsync(string mchNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.MchNo.Equals(mchNo));
        }

        public Task<bool> IsExistOrderUseWayCodeAsync(string wayCode)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.WayCode.Equals(wayCode));
        }

        public Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchOrderNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.MchNo.Equals(mchNo) && c.MchOrderNo.Equals(mchOrderNo));
        }
    }
}
