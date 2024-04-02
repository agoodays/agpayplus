using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayInterfaceConfigRepository : IAgPayRepository<PayInterfaceConfig, long>
    {
        bool IsExistUseIfCode(string ifCode);
        bool MchAppHasAvailableIfCode(string appId, string ifCode);
        void RemoveByInfoIds(List<string> infoIds, string infoType);
    }
}
