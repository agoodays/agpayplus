using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAccountBillService : IAgPayService<AccountBillDto>
    {
        Task GenAccountBillAsync(string payOrderId);
        Task<PaginatedResult<AccountBillDto>> GetPaginatedDataAsync(AccountBillQueryDto dto);
    }
}
