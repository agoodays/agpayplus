using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchNotifyRecordService : IAgPayService<MchNotifyRecordDto, long>
    {
        Task<PaginatedList<MchNotifyRecordDto>> GetPaginatedDataAsync(MchNotifyQueryDto dto);
        MchNotifyRecordDto FindByOrderAndType(string orderId, byte orderType);
        MchNotifyRecordDto FindByPayOrder(string payOrderId);
        MchNotifyRecordDto FindByRefundOrder(string payOrderId);
        MchNotifyRecordDto FindByTransferOrder(string payOrderId);
        int UpdateNotifyResult(long notifyId, byte state, string resResult);
        void UpdateIngAndAddNotifyCountLimit(long notifyId);
    }
}
