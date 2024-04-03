using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchStoreService : IAgPayService<MchStoreDto>
    {
        MchStoreDto GetById(long recordId, string mchNo);
        MchStoreDto GetByIdAsNoTracking(long recordId);
        IEnumerable<MchStoreDto> GetByMchNo(string mchNo);
        PaginatedList<MchStoreListDto> GetPaginatedData(MchStoreQueryDto dto, List<long> storeIds = null);
    }
}
