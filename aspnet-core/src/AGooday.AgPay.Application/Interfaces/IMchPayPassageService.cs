using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchPayPassageService : IAgPayService<MchPayPassageDto, long>
    {
        Task<bool> IsExistMchPayPassageUseWayCodeAsync(string wayCode);
        IEnumerable<MchPayPassageDto> GetMchPayPassageByMchNoAndAppId(string mchNo, string appId);
        IEnumerable<MchPayPassageDto> GetByAppIdAndWayCodesAsNoTracking(string appId, List<string> wayCodes);
        Task<PaginatedList<AvailablePayInterfaceDto>> SelectAvailablePayInterfaceListAsync(string wayCode, string appId, string infoType, byte mchType, int pageNumber, int pageSize);
        Task<bool> SetMchPassageAsync(string mchNo, string appId, string wayCode, string ifCode, byte state);
        Task<bool> SaveOrUpdateBatchSelfAsync(List<MchPayPassageDto> mchPayPassages, string mchNo);
        Task<MchPayPassageDto> FindMchPayPassageAsync(string mchNo, string appId, string wayCode);
        Task<MchPayPassageDto> FindMchPayPassageAsync(string mchNo, string appId, string wayCode, long amount, string bankCardType = null);
    }
}
