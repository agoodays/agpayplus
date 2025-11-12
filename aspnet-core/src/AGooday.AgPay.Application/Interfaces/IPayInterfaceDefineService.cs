using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceDefineService : IAgPayService<PayInterfaceDefineDto>
    {
        Task<IEnumerable<PayInterfaceDefineDto>> PayIfDefineListAsync(byte? state);
        Task<IEnumerable<PayInterfaceDefineDto>> GetByIfCodesAsync(IEnumerable<string> ifCodes);
    }
}
