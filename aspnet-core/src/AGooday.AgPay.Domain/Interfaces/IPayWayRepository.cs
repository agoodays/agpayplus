using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayWayRepository : IRepository<PayWay>
    {
        bool IsExistPayWayCode(string wayCode);
    }
}
