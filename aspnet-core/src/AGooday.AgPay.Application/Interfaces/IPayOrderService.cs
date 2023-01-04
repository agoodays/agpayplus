using AGooday.AgPay.Application.DataTransfer;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderService : IDisposable
    {
        bool Add(PayOrderDto dto);
        bool Remove(string recordId);
        bool Update(PayOrderDto dto);
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
        int UpdateOrderExpired();
        long CalMchIncomeAmount(PayOrderDto payOrder);
        JObject MainPageWeekCount(string mchNo, string agentNo);
        JObject MainPageNumCount(string mchNo, string agentNo);
        List<Dictionary<string, object>> MainPagePayCount(string mchNo, string agentNo, string createdStart, string createdEnd);
        List<PayTypeCountDto> MainPagePayTypeCount(string mchNo, string agentNo, string createdStart, string createdEnd);
    }
}
