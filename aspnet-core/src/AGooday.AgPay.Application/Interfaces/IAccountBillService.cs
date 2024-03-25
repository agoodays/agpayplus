using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAccountBillService : IDisposable
    {
        void GenAccountBill(string payOrderId);
        bool Add(AccountBillDto dto);
        bool Remove(long id);
        bool Update(AccountBillDto dto);
        AccountBillDto GetById(long id);
        IEnumerable<AccountBillDto> GetAll();
        PaginatedList<T> GetPaginatedData<T>(AccountBillQueryDto dto);
    }
}
