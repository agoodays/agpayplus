using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderProfitService : IAgPayService<PayOrderProfitDto>
    {
        Task<IEnumerable<PayOrderProfitDto>> GetByPayOrderIdAsNoTrackingAsync(string payOrderId);
        Task<IEnumerable<PayOrderProfitDto>> GetByPayOrderIdsAsNoTrackingAsync(List<string> payOrderIds);
    }
}
