using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IQrCodeService : IAgPayService<QrCodeDto>
    {
        Task<QrCodeDto> GetByIdAsNoTrackingAsync(string recordId);
        Task<string> BatchIdDistinctCountAsync();
        Task<bool> BatchAddAsync(QrCodeAddDto dto);
        Task<bool> UnBindAsync(string recordId);
        Task<PaginatedList<QrCodeDto>> GetPaginatedDataAsync(QrCodeQueryDto dto);
    }
}
