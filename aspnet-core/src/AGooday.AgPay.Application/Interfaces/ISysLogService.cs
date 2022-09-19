using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysLogService : IDisposable
    {
        void Add(SysLogDto dto);
        void Remove(long recordId);
        void Update(SysLogDto dto);
        SysLogDto GetById(long recordId);
        IEnumerable<SysLogDto> GetAll();
    }
}
