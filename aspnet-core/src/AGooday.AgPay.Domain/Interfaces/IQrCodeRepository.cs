using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IQrCodeRepository : IAgPayRepository<QrCode>
    {
        Task<bool> IsExistBatchIdAsync(string batchId);
        Task<QrCode> GetByIdAsNoTrackingAsync(string recordId);
    }
}
