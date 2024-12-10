using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverService : IAgPayService<MchDivisionReceiverDto, long>
    {
        MchDivisionReceiverDto GetById(long recordId, string mchNo);
        int GetCount(HashSet<long> receiverIds, string mchNo, string appId, string ifCode, byte state = CS.YES);
        int GetCount(HashSet<long> receiverGroupIds, string mchNo);
        bool IsExistUseReceiverGroup(long receiverGroupId);
        List<MchDivisionReceiverDto> GetAllMchReceiver(MchDivisionReceiverQueryDto dto);
        Task<PaginatedList<MchDivisionReceiverDto>> GetPaginatedDataAsync(MchDivisionReceiverQueryDto dto);
    }
}
