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
        Task<List<MchStoreDto>> GetByMchNoAsNoTrackingAsync(string mchNo);
        Task<List<MchStoreDto>> GetByStoreIdsAsNoTrackingAsync(IEnumerable<long?> storeIds);
        Task<PaginatedResult<MchStoreListDto>> GetPaginatedDataAsync(MchStoreQueryDto dto, List<long> storeIds = null);
    }
}
