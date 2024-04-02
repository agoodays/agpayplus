using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IRefundOrderRepository : IAgPayRepository<RefundOrder>
    {
        bool IsExistOrderByMchOrderNo(string mchNo, string mchRefundNo);
        bool IsExistRefundingOrder(string payOrderId);
    }
}
