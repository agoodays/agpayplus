using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IQrCodeShellService : IAgPayService<QrCodeShellDto>
    {
        Task<PaginatedList<QrCodeShellDto>> GetPaginatedDataAsync(QrCodeShellQueryDto dto);
    }
}
