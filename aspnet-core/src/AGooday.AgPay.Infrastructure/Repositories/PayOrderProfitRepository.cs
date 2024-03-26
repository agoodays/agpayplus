using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayOrderProfitRepository : Repository<PayOrderProfit>, IPayOrderProfitRepository
    {
        public PayOrderProfitRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public IQueryable<PayOrderProfit> GetByPayOrderIdAsNoTracking(string payOrderId)
        {
            return GetAllAsNoTracking().Where(w => w.PayOrderId.Equals(payOrderId));
        }
    }
}
