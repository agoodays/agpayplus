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
        bool IsExistOrderUseIfCode(string ifCode);
        bool IsExistOrderUseWayCode(string wayCode);
        PaginatedList<PayOrderDto> GetPaginatedData(PayOrderQueryDto dto);
        bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo);
        bool UpdateInit2Ing(string payOrderId, PayOrderDto payOrder);
        bool UpdateIng2SuccessOrFail(string payOrderId, byte state, string channelOrderId, string channelUserId, string channelErrCode, string channelErrMsg);
        bool UpdateDivisionState(PayOrderDto payOrder);
    }
}
