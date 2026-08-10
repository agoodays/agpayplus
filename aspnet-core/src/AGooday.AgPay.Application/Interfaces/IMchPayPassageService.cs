using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchPayPassageService : IAgPayService<MchPayPassageDto, long>
    {
        Task<bool> IsExistMchPayPassageUseWayCodeAsync(string wayCode);
        IEnumerable<MchPayPassageDto> GetMchPayPassageByMchNoAndAppId(string mchNo, string appId);
        IEnumerable<MchPayPassageDto> GetByAppIdAndWayCodesAsNoTracking(string appId, List<string> wayCodes);
        Task<PaginatedResult<AvailablePayInterfaceDto>> SelectAvailablePayInterfaceListAsync(string appId, string wayCode, string infoType, byte mchType, byte? state, int pageNumber, int pageSize);
        Task<bool> SetMchPassageAsync(string mchNo, string appId, string wayCode, string ifCode, byte state);
        Task<bool> SaveOrUpdateBatchSelfAsync(List<MchPayPassageDto> mchPayPassages, string mchNo);
        Task<MchPayPassageDto> FindMchPayPassageAsync(string mchNo, string appId, string wayCode);
        Task<MchPayPassageDto> FindMchPayPassageAsync(string mchNo, string appId, string wayCode, long amount, string bankCardType = null);

        /// <summary>
        /// 查询应用支付接口配置列表（支付方式 + 通道配置状态）。
        /// 无 PassageState/IsConfig 过滤时走 SQL 分页；
        /// 有虚拟字段过滤时查全量 PayWay → 计算虚拟字段 → 内存过滤 → 手动分页。
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="dto">分页查询参数</param>
        /// <param name="mchNo">商户号（可选，Merchant 场景传入用于通道过滤）</param>
        Task<PaginatedResult<MchPayPassagePayWayDto>> GetConfiguredPayWayPageListAsync(string appId, PayWayQueryDto dto, string mchNo = null);
    }
}
