using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayOrderProfitRepository : AgPayRepository<PayOrderProfit>, IPayOrderProfitRepository
    {
        public PayOrderProfitRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public IQueryable<PayOrderProfit> GetByPayOrderIdAsNoTracking(string payOrderId)
        {
            return GetAllAsNoTracking().Where(w => w.PayOrderId.Equals(payOrderId));
        }

        public IQueryable<PayOrderProfit> GetByPayOrderIdsAsNoTracking(List<string> payOrderIds)
        {
            return GetAllAsNoTracking().Where(w => payOrderIds.Contains(w.PayOrderId));
        }
    }
}
