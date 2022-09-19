using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayWayService : IDisposable
    {
        void Add(PayWayDto dto);
        void Remove(string recordId);
        void Update(PayWayDto dto);
        PayWayDto GetById(string recordId);
        IEnumerable<PayWayDto> GetAll();
    }
}
