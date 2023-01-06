using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchStoreService : IDisposable
    {
        bool Add(MchStoreDto dto);
        bool Remove(long recordId);
        bool Update(MchStoreDto dto);
        MchStoreDto GetById(long recordId);
        MchStoreDto GetById(long recordId, string mchNo);
        IEnumerable<MchStoreDto> GetAll();
        PaginatedList<MchStoreListDto> GetPaginatedData(MchStoreQueryDto dto, string agentNo = null);
    }
}
