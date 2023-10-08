using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceDefineService : IDisposable
    {
        bool Add(PayInterfaceDefineDto dto);
        bool Remove(string recordId);
        bool Update(PayInterfaceDefineDto dto);
        PayInterfaceDefineDto GetById(string recordId);
        IEnumerable<PayInterfaceDefineDto> GetByIfCodes(IEnumerable<string> ifCodes);
        IEnumerable<PayInterfaceDefineDto> GetAll();
    }
}
