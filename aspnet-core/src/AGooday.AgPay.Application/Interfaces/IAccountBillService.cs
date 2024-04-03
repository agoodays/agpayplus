using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAccountBillService : IAgPayService<AccountBillDto>
    {
        void GenAccountBill(string payOrderId);
        PaginatedList<T> GetPaginatedData<T>(AccountBillQueryDto dto);
    }
}
