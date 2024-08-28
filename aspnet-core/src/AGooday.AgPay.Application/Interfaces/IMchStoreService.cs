using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchStoreService : IAgPayService<MchStoreDto>
    {
        MchStoreDto GetById(long recordId, string mchNo);
        Task<MchStoreDto> GetByIdAsync(long recordId, string mchNo);
        Task<MchStoreDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo);
        Task<MchStoreDto> GetByIdAsNoTrackingAsync(long recordId);
        IEnumerable<MchStoreDto> GetByMchNoAsNoTracking(string mchNo);
        IEnumerable<MchStoreDto> GetByStoreIdsAsNoTracking(IEnumerable<long?> storeIds);
        PaginatedList<MchStoreListDto> GetPaginatedData(MchStoreQueryDto dto, List<long> storeIds = null);
    }
}
