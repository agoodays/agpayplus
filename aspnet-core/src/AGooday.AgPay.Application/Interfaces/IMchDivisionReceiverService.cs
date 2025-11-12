using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverService : IAgPayService<MchDivisionReceiverDto, long>
    {
        Task<MchDivisionReceiverDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo);
        Task<int> GetCountAsync(HashSet<long> receiverIds, string mchNo, string appId, string ifCode, byte state = CS.YES);
        Task<int> GetCountAsync(HashSet<long> receiverGroupIds, string mchNo);
        Task<bool> IsExistUseReceiverGroupAsync(long receiverGroupId);
        Task<PaginatedResult<MchDivisionReceiverDto>> GetPaginatedDataAsync(MchDivisionReceiverQueryDto dto);
    }
}
