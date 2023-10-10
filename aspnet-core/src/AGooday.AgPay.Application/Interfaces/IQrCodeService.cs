using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IQrCodeService : IDisposable
    {
        string BatchIdDistinctCount();
        bool BatchAdd(QrCodeAddDto dto);
        bool Add(QrCodeDto dto);
        bool Remove(string recordId);
        bool Update(QrCodeDto dto);
        QrCodeDto GetById(string recordId);
        IEnumerable<QrCodeDto> GetAll();
        PaginatedList<T> GetPaginatedData<T>(QrCodeQueryDto dto);
    }
}
