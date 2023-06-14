using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IQrCodeShellService : IDisposable
    {
        bool Add(QrCodeShellDto dto);
        bool Remove(long recordId);
        bool Update(QrCodeShellDto dto);
        QrCodeShellDto GetById(long recordId);
        IEnumerable<QrCodeShellDto> GetAll();
        PaginatedList<T> GetPaginatedData<T>(QrCodeShellQueryDto dto);
    }
}
