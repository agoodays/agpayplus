using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchAppRepository : IRepository<MchApp>
    {
        MchApp GetAsNoTrackingById(string id);
    }
}
