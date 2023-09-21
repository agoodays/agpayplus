using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchStoreRepository : IRepository<MchStore>
    {
        MchStore GetByKeyAsNoTracking(long recordId);
    }
}
