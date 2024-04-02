using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchAppRepository : IAgPayRepository<MchApp>
    {
        MchApp GetAsNoTrackingById(string id);
    }
}
