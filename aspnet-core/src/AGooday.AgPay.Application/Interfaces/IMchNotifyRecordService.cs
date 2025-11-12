using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchNotifyRecordService : IAgPayService<MchNotifyRecordDto, long>
    {
        Task<PaginatedResult<MchNotifyRecordDto>> GetPaginatedDataAsync(MchNotifyQueryDto dto);
        Task<MchNotifyRecordDto> FindByOrderAndTypeAsync(string orderId, byte orderType);
        Task<MchNotifyRecordDto> FindByPayOrderAsync(string payOrderId);
        Task<MchNotifyRecordDto> FindByRefundOrderAsync(string payOrderId);
        Task<MchNotifyRecordDto> FindByTransferOrder(string payOrderId);
        Task<int> UpdateNotifyResultAsync(long notifyId, byte state, string resResult);
        Task<int> UpdateIngAndAddNotifyCountLimitAsync(long notifyId);
    }
}
