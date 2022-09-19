using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderDivisionRecordService : IDisposable
    {
        void Add(PayOrderDivisionRecordDto dto);
        void Remove(long recordId);
        void Update(PayOrderDivisionRecordDto dto);
        PayOrderDivisionRecordDto GetById(long recordId);
        IEnumerable<PayOrderDivisionRecordDto> GetAll();
    }
}
