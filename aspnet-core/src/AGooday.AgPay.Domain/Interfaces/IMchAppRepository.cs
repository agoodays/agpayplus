using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchAppRepository : IAgPayRepository<MchApp>
    {
        Task<MchApp> GetByIdAsNoTrackingAsync(string recordId);
        Task<MchApp> GetByIdAsync(string recordId, string mchNo);
        Task<MchApp> GetByIdAsNoTrackingAsync(string recordId, string mchNo);
    }
}
