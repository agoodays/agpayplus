using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IQrCodeRepository : IAgPayRepository<QrCode>
    {
        QrCode GetByIdAsNoTracking(string recordId);
    }
}
