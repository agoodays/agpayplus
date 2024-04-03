using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceDefineService : IAgPayService<PayInterfaceDefineDto>
    {
        IEnumerable<PayInterfaceDefineDto> GetByIfCodes(IEnumerable<string> ifCodes);
    }
}
