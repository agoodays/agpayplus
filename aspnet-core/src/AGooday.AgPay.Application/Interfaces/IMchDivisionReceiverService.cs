using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverService : IDisposable
    {
        bool Add(MchDivisionReceiverDto dto);
        bool Remove(long recordId);
        bool Update(MchDivisionReceiverDto dto);
        MchDivisionReceiverDto GetById(long recordId);
        MchDivisionReceiverDto GetById(long recordId, string mchNo);
        IEnumerable<MchDivisionReceiverDto> GetAll();
        bool IsExistUseReceiverGroup(long receiverGroupId);
        PaginatedList<MchDivisionReceiverDto> GetPaginatedData(MchDivisionReceiverQueryDto dto);
    }
}
