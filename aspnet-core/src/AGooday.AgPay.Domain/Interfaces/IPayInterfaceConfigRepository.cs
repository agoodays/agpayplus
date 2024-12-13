using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayInterfaceConfigRepository : IAgPayRepository<PayInterfaceConfig, long>
    {
        Task<bool> IsExistUseIfCodeAsync(string ifCode);
        Task<bool> MchAppHasAvailableIfCodeAsync(string appId, string ifCode);
        void RemoveByInfoIds(IQueryable<string> infoIds, string infoType);
    }
}
