using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverService : IDisposable
    {
        bool Add(MchDivisionReceiverDto dto);
        bool Remove(long recordId);
        bool Update(MchDivisionReceiverDto dto);
        MchDivisionReceiverDto GetById(long recordId);
        MchDivisionReceiverDto GetById(long recordId, string mchNo);
        int GetCount(HashSet<long> receiverIds, string mchNo, string appId, string ifCode, byte state = CS.YES);
        int GetCount(HashSet<long> receiverGroupIds, string mchNo);
        IEnumerable<MchDivisionReceiverDto> GetAll();
        bool IsExistUseReceiverGroup(long receiverGroupId);
        PaginatedList<MchDivisionReceiverDto> GetPaginatedData(MchDivisionReceiverQueryDto dto);
    }
}
