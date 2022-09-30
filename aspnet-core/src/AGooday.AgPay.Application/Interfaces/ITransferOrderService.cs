using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ITransferOrderService : IDisposable
    {
        void Add(TransferOrderDto dto);
        void Remove(string recordId);
        void Update(TransferOrderDto dto);
        TransferOrderDto GetById(string recordId);
        IEnumerable<TransferOrderDto> GetAll();
        PaginatedList<TransferOrderDto> GetPaginatedData(TransferOrderQueryDto dto);
    }
}
