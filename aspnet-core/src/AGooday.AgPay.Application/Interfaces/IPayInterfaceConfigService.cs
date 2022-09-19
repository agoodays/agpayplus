using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceConfigService : IDisposable
    {
        void Add(PayInterfaceConfigDto dto);
        void Remove(long recordId);
        void Update(PayInterfaceConfigDto dto);
        PayInterfaceConfigDto GetById(long recordId);
        IEnumerable<PayInterfaceConfigDto> GetAll();
    }
}
