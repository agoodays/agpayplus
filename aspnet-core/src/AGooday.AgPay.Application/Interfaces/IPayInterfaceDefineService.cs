using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceDefineService : IDisposable
    {
        void Add(PayInterfaceDefineDto dto);
        void Remove(string recordId);
        void Update(PayInterfaceDefineDto dto);
        PayInterfaceDefineDto GetById(string recordId);
        IEnumerable<PayInterfaceDefineDto> GetAll();
    }
}
