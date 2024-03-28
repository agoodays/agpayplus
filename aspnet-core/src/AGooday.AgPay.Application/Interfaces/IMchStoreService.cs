using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchStoreService : IDisposable
    {
        bool Add(MchStoreDto dto);
        bool Remove(long recordId);
        bool Update(MchStoreDto dto);
        MchStoreDto GetById(long recordId);
        MchStoreDto GetById(long recordId, string mchNo);
        MchStoreDto GetByIdAsNoTracking(long recordId);
        IEnumerable<MchStoreDto> GetByMchNo(string mchNo);
        IEnumerable<MchStoreDto> GetAll();
        PaginatedList<MchStoreListDto> GetPaginatedData(MchStoreQueryDto dto, string agentNo = null);
    }
}
