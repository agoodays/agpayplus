using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IRefundOrderService : IDisposable
    {
        void Add(RefundOrderDto dto);
        void Remove(string recordId);
        void Update(RefundOrderDto dto);
        RefundOrderDto GetById(string recordId);
        IEnumerable<RefundOrderDto> GetAll();
    }
}
