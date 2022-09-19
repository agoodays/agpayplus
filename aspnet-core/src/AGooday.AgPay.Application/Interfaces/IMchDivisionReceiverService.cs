using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchDivisionReceiverService : IDisposable
    {
        void Add(MchDivisionReceiverDto dto);
        void Remove(long recordId);
        void Update(MchDivisionReceiverDto dto);
        MchDivisionReceiverDto GetById(long recordId);
        IEnumerable<MchDivisionReceiverDto> GetAll();
    }
}
