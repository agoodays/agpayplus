using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderProfitService : IAgPayService<PayOrderProfitDto>
    {
        IEnumerable<PayOrderProfitDto> GetByPayOrderIdAsNoTracking(string payOrderId);
        IEnumerable<PayOrderProfitDto> GetByPayOrderIdsAsNoTracking(List<string> payOrderIds);
    }
}
