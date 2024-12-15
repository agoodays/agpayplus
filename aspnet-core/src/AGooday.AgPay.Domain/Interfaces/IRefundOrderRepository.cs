using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IRefundOrderRepository : IAgPayRepository<RefundOrder>
    {
        Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchRefundNo);
        Task<bool> IsExistRefundingOrderAsync(string payOrderId);
    }
}
