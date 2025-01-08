using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchStoreRepository : IAgPayRepository<MchStore>
    {
        Task<MchStore> GetByIdAsNoTrackingAsync(long recordId);
        MchStore GetById(long recordId, string mchNo);
        Task<MchStore> GetByIdAsync(long recordId, string mchNo);
        Task<MchStore> GetByIdAsNoTrackingAsync(long recordId, string mchNo);
    }
}
