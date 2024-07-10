using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IQrCodeService : IAgPayService<QrCodeDto>
    {
        QrCodeDto GetByIdAsNoTracking(string recordId);
        string BatchIdDistinctCount();
        bool BatchAdd(QrCodeAddDto dto);
        PaginatedList<QrCodeDto> GetPaginatedData(QrCodeQueryDto dto);
    }
}
