using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayOrderRepository : IAgPayRepository<PayOrder>
    {
        Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchOrderNo);
        Task<bool> IsExistOrderUseIfCodeAsync(string ifCode);
        Task<bool> IsExistOrderUseMchNoAsync(string mchNo);
        Task<bool> IsExistOrderUseWayCodeAsync(string wayCode);
    }
}
