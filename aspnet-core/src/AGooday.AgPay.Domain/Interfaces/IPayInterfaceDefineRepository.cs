using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayInterfaceDefineRepository : IRepository<PayInterfaceDefine>
    {
        IEnumerable<T> SelectAvailablePayInterfaceList<T>(string wayCode, string appId, string infoType, byte mchType);
    }
}
