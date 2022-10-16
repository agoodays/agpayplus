using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderService : IDisposable
    {
        void Add(PayOrderDto dto);
        void Remove(string recordId);
        void Update(PayOrderDto dto);
        PayOrderDto GetById(string recordId);
        IEnumerable<PayOrderDto> GetAll();
        PayOrderDto QueryMchOrder(string mchNo, string payOrderId, string mchOrderNo);
        PaginatedList<PayOrderDto> GetPaginatedData(PayOrderQueryDto dto);
        bool IsExistOrderUseIfCode(string ifCode);
        bool IsExistOrderUseWayCode(string wayCode);
        bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo);
        bool UpdateDivisionState(PayOrderDto payOrder);
        bool UpdateInit2Ing(string payOrderId, PayOrderDto payOrder);
        bool UpdateIng2SuccessOrFail(string payOrderId, byte state, string channelOrderId, string channelUserId, string channelErrCode, string channelErrMsg);
        bool UpdateNotifySent(string orderId);
        bool UpdateIng2Success(string payOrderId, string channelOrderId, string channelUserId);
        bool UpdateIng2Fail(string payOrderId, string channelOrderId, string channelUserId, string channelErrCode, string channelErrMsg);
    }
}
