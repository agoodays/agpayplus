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
        bool Add(PayOrderDivisionRecordDto dto);
        bool Remove(long recordId);
        bool Update(PayOrderDivisionRecordDto dto);
        PayOrderDivisionRecordDto GetById(long recordId);
        PayOrderDivisionRecordDto GetById(long recordId, string mchNo);
        IEnumerable<PayOrderDivisionRecordDto> GetAll();
        PaginatedList<PayOrderDivisionRecordDto> GetPaginatedData(PayOrderDivisionRecordQueryDto dto);
        void UpdateRecordSuccessOrFail(List<PayOrderDivisionRecordDto> records, byte state, string channelBatchOrderId, string channelRespResult);
    }
}
