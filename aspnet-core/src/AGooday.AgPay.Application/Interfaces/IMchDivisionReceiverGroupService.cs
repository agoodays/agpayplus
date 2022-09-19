using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverGroupService : IDisposable
    {
        void Add(MchDivisionReceiverGroupDto dto);
        void Remove(long recordId);
        void Update(MchDivisionReceiverGroupDto dto);
        MchDivisionReceiverGroupDto GetById(long recordId);
        IEnumerable<MchDivisionReceiverGroupDto> GetAll();
    }
}
