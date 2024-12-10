using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverGroupService : IAgPayService<MchDivisionReceiverGroupDto, long>
    {
        MchDivisionReceiverGroupDto GetById(long recordId, string mchNo);
        IEnumerable<MchDivisionReceiverGroupDto> GetByMchNo(string mchNo);
        MchDivisionReceiverGroupDto FindByIdAndMchNo(long receiverGroupId, string mchNo);
        Task<PaginatedList<MchDivisionReceiverGroupDto>> GetPaginatedDataAsync(MchDivisionReceiverGroupQueryDto dto);
    }
}
