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
        PaginatedList<RefundOrderDto> GetPaginatedData(RefundOrderQueryDto dto);
        long SumSuccessRefundAmount(string payOrderId);
        bool UpdateInit2Ing(string refundOrderId);
        bool UpdateIng2SuccessOrFail(string refundOrderId, byte state, string channelOrderId, string channelErrCode, string channelErrMsg);
        bool IsExistOrderByMchOrderNo(string mchNo, string mchRefundNo);
        bool IsExistRefundingOrder(string payOrderId);
    }
}
