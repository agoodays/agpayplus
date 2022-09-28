using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchNotifyRecordService : IDisposable
    {
        void Add(MchNotifyRecordDto dto);
        void Remove(long recordId);
        void Update(MchNotifyRecordDto dto);
        MchNotifyRecordDto GetById(long recordId);
        IEnumerable<MchNotifyRecordDto> GetAll();
        PaginatedList<MchNotifyRecordDto> GetPaginatedData(MchNotifyQueryDto dto);
        void UpdateIngAndAddNotifyCountLimit(long notifyId);
    }
}
