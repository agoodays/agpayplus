using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAccountBillService : IDisposable
    {
        void GenAccountBill(string payOrderId);
        bool Add(AccountBillDto dto);
        bool Remove(string recordId);
        bool Update(AccountBillDto dto);
        AccountBillDto GetById(string recordId);
        IEnumerable<AccountBillDto> GetAll();
        PaginatedList<T> GetPaginatedData<T>(AccountBillQueryDto dto);
    }
}
