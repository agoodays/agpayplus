using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverGroupService : IAgPayService<MchDivisionReceiverGroupDto, long>
    {
        Task UpdateAutoDivisionFlagAsync(MchDivisionReceiverGroupDto dto);
        Task<MchDivisionReceiverGroupDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo);
        IEnumerable<MchDivisionReceiverGroupDto> GetByMchNo(string mchNo);
        Task<MchDivisionReceiverGroupDto> FindByIdAndMchNoAsync(long receiverGroupId, string mchNo);
        Task<PaginatedResult<MchDivisionReceiverGroupDto>> GetPaginatedDataAsync(MchDivisionReceiverGroupQueryDto dto);
    }
}
