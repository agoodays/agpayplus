using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayWayRepository : IAgPayRepository<PayWay>
    {
        Task<bool> IsExistPayWayCodeAsync(string wayCode);
    }
}
