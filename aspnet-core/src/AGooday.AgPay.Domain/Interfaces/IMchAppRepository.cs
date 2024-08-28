using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchAppRepository : IAgPayRepository<MchApp>
    {
        MchApp GetByIdAsNoTracking(string recordId);
        MchApp GetById(string recordId, string mchNo);
        MchApp GetByIdAsNoTracking(string recordId, string mchNo);
    }
}
