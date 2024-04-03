using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IQrCodeService : IAgPayService<QrCodeDto>
    {
        string BatchIdDistinctCount();
        bool BatchAdd(QrCodeAddDto dto);
        PaginatedList<T> GetPaginatedData<T>(QrCodeQueryDto dto);
    }
}
