using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverGroupService : IDisposable
    {
        bool Add(MchDivisionReceiverGroupDto dto);
        bool Remove(long recordId);
        bool Update(MchDivisionReceiverGroupDto dto);
        MchDivisionReceiverGroupDto GetById(long recordId);
        MchDivisionReceiverGroupDto GetById(long recordId, string mchNo);
        IEnumerable<MchDivisionReceiverGroupDto> GetAll();
        IEnumerable<MchDivisionReceiverGroupDto> GetByMchNo(string mchNo);
        MchDivisionReceiverGroupDto FindByIdAndMchNo(long receiverGroupId, string mchNo);
        PaginatedList<MchDivisionReceiverGroupDto> GetPaginatedData(MchDivisionReceiverGroupQueryDto dto);
    }
}
