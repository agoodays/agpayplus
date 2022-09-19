using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysConfigService : IDisposable
    {
        void Add(SysConfigDto dto);
        void Remove(string recordId);
        void Update(SysConfigDto dto);
        SysConfigDto GetById(string recordId);
        IEnumerable<SysConfigDto> GetAll();
    }
}
