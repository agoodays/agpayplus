using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchStoreRepository : IAgPayRepository<MchStore>
    {
        MchStore GetByIdAsNoTracking(long recordId);
    }
}
