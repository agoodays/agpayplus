using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayOrderProfitRepository : IRepository<PayOrderProfit>
    {
        IQueryable<PayOrderProfit> GetByPayOrderId(string payOrderId);
    }
}
