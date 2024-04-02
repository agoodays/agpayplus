using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayOrderProfitRepository : IAgPayRepository<PayOrderProfit>
    {
        IQueryable<PayOrderProfit> GetByPayOrderIdAsNoTracking(string payOrderId);
        IQueryable<PayOrderProfit> GetByPayOrderIdsAsNoTracking(List<string> payOrderIds);
    }
}
